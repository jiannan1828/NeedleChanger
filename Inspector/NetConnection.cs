using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

//20241031
namespace xNet
{
    #region RingBuffer
    public class RingBuffer
    {
        public byte[] Buffer;
        internal int rIndex = 0, wIndex = 0, dataCount = 0;
        internal object dLock = new object();

        public RingBuffer(int BufferSizeMB)
        { AdjustBuffer(BufferSizeMB); }

        public void AdjustBuffer(int BufferSizeMB)
        {
            lock (dLock)
            {
                Buffer = new byte[BufferSizeMB * 1024 * 1024];
                rIndex = wIndex = dataCount = 0;
            }
        }

        public int DataCount
        { get { return dataCount; } }

        public int EmptyCount
        { get { return Buffer.Length - dataCount; } }

        public void Clear()
        {
            lock (dLock)
                rIndex = wIndex = dataCount = 0;
        }

        public int Read(byte[] tagretBuffer, int Offset, int ReadCount)
        {
            int mCount = Math.Min(ReadCount, dataCount);
            int fCount = Math.Min(mCount, Buffer.Length - rIndex);
            int bCount = mCount - fCount;
            if (mCount > 0)
                lock (dLock)
                {
                    Array.Copy(Buffer, rIndex, tagretBuffer, Offset, fCount);
                    rIndex = (rIndex + fCount) % Buffer.Length;
                    dataCount -= fCount;
                    Offset += fCount;
                    if (bCount > 0)
                    {
                        Array.Copy(Buffer, rIndex, tagretBuffer, Offset, bCount);
                        rIndex = (rIndex + bCount) % Buffer.Length;
                        dataCount -= bCount;
                    }
                }
            return mCount;
        }

        public int Write(byte[] sourceBuffer, int Offset, int WriteCount)
        {
            int mCount = Math.Min(WriteCount, Buffer.Length - dataCount);
            int fCount = Math.Min(mCount, Buffer.Length - wIndex);
            int bCount = mCount - fCount;
            if (mCount > 0)
                lock (dLock)
                {
                    Array.Copy(sourceBuffer, Offset, Buffer, wIndex, fCount);
                    wIndex = (wIndex + fCount) % Buffer.Length;
                    dataCount += fCount;
                    Offset += fCount;
                    if (bCount > 0)
                    {
                        Array.Copy(sourceBuffer, Offset, Buffer, wIndex, bCount);
                        wIndex = (wIndex + bCount) % Buffer.Length;
                        dataCount += bCount;
                    }
                }
            return mCount;
        }
        /// <summary>回傳包含Item的總數量</summary>
        public int IndexOf(byte Item)
        {
            if (dataCount < 1)
                return 0;
            int index = 0;
            int fCount = Math.Min(dataCount, Buffer.Length - rIndex);
            int bCount = dataCount - fCount;
            if (fCount > 0)
            {
                index = Array.IndexOf(Buffer, Item, rIndex, fCount);
                index = (index < 0) ? 0 : index - rIndex + 1;
            }
            if ((index < 1) && (bCount > 0))
            {
                index = Array.IndexOf(Buffer, Item, 0, bCount);
                index = (index < 0) ? 0 : index + fCount + 1;
            }
            return index;
        }

        public int FindIndex(byte[] source, byte[] item, int StartIndex, int Count)
        {
            int Index = -1;
            int bStart = StartIndex, bCount = Count;
            while ((bCount >= item.Length) && ((Index = Array.IndexOf(source, item[0], bStart, bCount)) >= 0))
            {
                bCount = Count + StartIndex - bStart - 1;
                bStart = Index + 1;
                if (bCount >= (item.Length - 1))
                {
                    bool Finded = true;
                    for (int i = 1; i < item.Length; i++)
                        if (source[bStart + i - 1] != item[i])
                        {
                            Finded = false;
                            break;
                        }
                    if (Finded)
                        break;
                }
                else
                {
                    Index = -1;
                    break;
                }
            }
            return Index;
        }

    }
    #endregion

    #region xRingTCP
#pragma warning disable CS0168
    public class xRingTCP : IDisposable
    {
        Socket Local;
        byte[] TempBuf = new byte[1024];
        RingBuffer RecvBuffer;
        int BufferSize = 0;
        object LockClient = new object();
        [DllImport("kernel32.dll")]
        static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
        bool AutoReconnect = false, Sending = false;
        string DestIP = "", NetName = "";
        int DestPort = 0, MaxClients = 64;
        Task _doConnect = null, _doListen = null, _doAccept = null, _doRecv = null;
        double SendIDLE = 0;
        IAsyncResult asyncResult = null;
        bool disposed = false;
        public List<xRingTCP> Clients = new List<xRingTCP>();
        public bool Connected
        {
            get { return (Local != null) && Local.Connected; }
        }
        public int DataCount
        { get { return RecvBuffer.DataCount; } }
        double NowTime()
        { return DateTime.Now.Ticks / 10000000.0; }
        /// <summary>建立新連線，指定自動收資料大小(MB)</summary>
        public xRingTCP(string netName, int AutoReceiveSize)
        {
            NetName = netName;
            BufferSize = AutoReceiveSize;
            RecvBuffer = new RingBuffer(BufferSize);
        }
        /// <summary>建立已連線對象，Server用</summary>
        public xRingTCP(Socket target, int AutoReceiveSize)
        {
            Local = target;
            Local.LingerState = new LingerOption(true, 1);
            BufferSize = AutoReceiveSize;
            RecvBuffer = new RingBuffer(BufferSize);
            Task.Factory.StartNew(TryRecv);
        }

        public void ConnectTo(string IP, int Port, bool autoReconnect)
        {
            AutoReconnect = autoReconnect;
            DestIP = IP;
            DestPort = Port;
            RecvBuffer.Clear();
            if ((_doConnect == null) || _doConnect.IsCompleted)
                _doConnect = Task.Factory.StartNew(TryConnect);
        }

        void TryConnect()
        {
            using (Ping dPing = new Ping())
            {
                IPAddress tIP = IPAddress.Parse(DestIP);
                while (true)
                {
                    try
                    {
                        if (dPing.Send(tIP, 2).Status == IPStatus.Success)
                            break;
                    }
                    catch (Exception ex)
                    { }
                    Thread.Sleep(2000);
                }
            }
            try
            {
                Local = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Local.LingerState = new LingerOption(true, 1);
                //SetKeepAlive(true, 500);
                var ar = Local.BeginConnect(DestIP, DestPort, null, null);
                ar.AsyncWaitHandle.WaitOne();
                Local.EndConnect(ar);
            }
            catch (Exception ex)
            { }
            Task.Factory.StartNew(TryRecv);
        }

        void doRecv()
        {
            int mCount = Math.Min(RecvBuffer.Buffer.Length - RecvBuffer.wIndex, RecvBuffer.EmptyCount);
            try
            {
                var ar = Local.BeginReceive(RecvBuffer.Buffer, RecvBuffer.wIndex, mCount, SocketFlags.None, null, null);
                ar.AsyncWaitHandle.WaitOne();
                int Count = Local.EndReceive(ar);
                if (Count > 0)
                    lock (RecvBuffer.dLock)
                    {
                        RecvBuffer.wIndex = (RecvBuffer.wIndex + Count) % RecvBuffer.Buffer.Length;
                        RecvBuffer.dataCount += Count;
                    }
            }
            catch (Exception ex)
            { }
        }

        void CheckSendIdle()
        {
            if (Sending)
                return;
            if ((NowTime() - SendIDLE) < 2)
                return;
            try
            {
                Local.BeginSend(TempBuf, 0, 0, SocketFlags.None, (ar) =>
                {
                    try
                    {
                        Local.EndSend(ar);
                    }
                    catch (Exception ex)
                    { }
                }, null);
            }
            catch (Exception ex)
            { }
            SendIDLE = NowTime();
        }

        void TryRecv()
        {
            Thread.Sleep(100);
            SendIDLE = NowTime();
            while (Connected)
            {
                Thread.Sleep(1);
                if (RecvBuffer.EmptyCount > 0)
                    if ((_doRecv == null) || _doRecv.IsCompleted)
                        _doRecv = Task.Factory.StartNew(doRecv);
                //CheckSendIdle();
            }
            Local.Close();
            RecvBuffer.Clear();
            Thread.Sleep(1000);
            if (AutoReconnect)
                ConnectTo(DestIP, DestPort, AutoReconnect);
        }
        /// <summary>由Buffer接收資料</summary>
        public int Receive(byte[] buffer, int Offset, int ReadCount)
        { return RecvBuffer.Read(buffer, Offset, ReadCount); }
        /// <summary>由Buffer接收字串資料</summary>
        public string Receive()
        {
            if (DataCount < 1)
                return "";
            byte[] Temp = new byte[DataCount];
            Receive(Temp, 0, Temp.Length);
            return Encoding.ASCII.GetString(Temp);
        }
        /// <summary>由Buffer接收分隔的字串資料</summary>
        public string Receive(byte split)
        {
            if (RecvBuffer.DataCount < 1)
                return "";
            int count = RecvBuffer.IndexOf(split);
            if (count < 1)
                return "";
            byte[] Temp = new byte[count];
            Receive(Temp, 0, Temp.Length);
            return Encoding.ASCII.GetString(Temp);
        }
        /// <summary>回傳包含Item的總數量</summary>
        public int IndexOf(byte split)
        { return RecvBuffer.IndexOf(split); }

        public int FindIndex(byte[] source, byte[] item, int StartIndex, int Count)
        { return RecvBuffer.FindIndex(source, item, StartIndex, Count); }
        /// <summary>異步發送資料</summary>
        void SendNoWait(byte[] buffer, int Offset, int SendCount)
        {
            SendIDLE = NowTime();
            try
            {
                if (asyncResult != null)
                    asyncResult.AsyncWaitHandle.WaitOne();
                asyncResult = Local.BeginSend(buffer, Offset, SendCount, SocketFlags.None, (ar) =>
                {
                    try
                    {
                        (ar.AsyncState as Socket).EndSend(ar);
                    }
                    catch (Exception ex)
                    { }
                    Sending = false;
                    SendIDLE = NowTime();
                }, Local);
                Sending = true;
            }
            catch (Exception ex)
            { }
        }
        /// <summary>異步發送資料</summary>
        void SendWait(byte[] buffer, int Offset, int SendCount)
        {
            SendIDLE = NowTime();
            try
            {
                Sending = true;
                var ar = Local.BeginSend(buffer, Offset, SendCount, SocketFlags.None, null, null);
                ar.AsyncWaitHandle.WaitOne();
            }
            catch (Exception ex)
            { }
            Sending = false;
            SendIDLE = NowTime();
        }
        /// <summary>異步發送資料</summary>
        public void Send(byte[] buffer, int Offset, int SendCount, bool wait)
        {
            if (wait)
                SendWait(buffer, Offset, SendCount);
            else
                SendNoWait(buffer, Offset, SendCount);
        }
        /// <summary>取消目前操作</summary>
        public void Cancel()
        { CancelIoEx(Local.Handle, IntPtr.Zero); }
        /// <summary>清除自動讀取的緩衝區內容</summary>
        public void ClearBuffer()
        { RecvBuffer.Clear(); }
        /// <summary>啟動Server端</summary>
        public void Listen(string localIP, int localPort, int maxClients)
        {
            DestIP = localIP;
            DestPort = localPort;
            MaxClients = maxClients;
            if ((_doListen == null) || _doListen.IsCompleted)
                _doListen = Task.Factory.StartNew(doListen);
        }

        void doAccept()
        {
            Thread.Sleep(50);
            try
            {
                var ar = Local.BeginAccept(null, null);
                ar.AsyncWaitHandle.WaitOne();
                Socket acct = Local.EndAccept(ar);
                lock (LockClient)
                    Clients.Add(new xRingTCP(acct, BufferSize) { NetName = NetName + "Sub" + Clients.Count.ToString() });
            }
            catch (Exception ex)
            { }
        }

        void doListen()
        {
            if (Local != null)
                Local.Close();
            Local = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Local.LingerState = new LingerOption(true, 1);
            bool Success = false;
            try
            {
                Local.Bind(new IPEndPoint(IPAddress.Parse(DestIP), DestPort));
                Local.Listen(MaxClients);
                Success = true;
            }
            catch (Exception ex)
            { }
            while (Success)
                try
                {
                    if ((_doAccept == null) || _doAccept.IsCompleted)
                        _doAccept = Task.Factory.StartNew(doAccept);
                    int temp = Local.Available; //主動斷線時使用
                    for (int i = Clients.Count - 1; i >= 0; i--)
                        if (!Clients[i].Connected)
                            lock (LockClient)
                                Clients.RemoveAt(i);
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                { Success = false; }
            lock (LockClient)
                Clients.Clear();
        }

        public void SetKeepAlive(bool enable, int interval_ms)
        {
            byte[] Temp = new byte[12];
            byte[] V = enable ? BitConverter.GetBytes(1) : BitConverter.GetBytes(0);
            Array.Copy(V, 0, Temp, 0, 4);
            V = BitConverter.GetBytes(1);
            Array.Copy(V, 0, Temp, 4, 4);
            V = BitConverter.GetBytes(interval_ms);
            Array.Copy(V, 0, Temp, 8, 4);
            Local.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, Temp);
        }

        public void Close()
        {
            AutoReconnect = false;
            try
            {
                foreach (var P in Clients)
                    P.Close();
                Local.Close();
            }
            catch (Exception ex)
            { }
        }

        #region IDisposable實作
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~xRingTCP() { Dispose(false); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Close();
            disposed = true;
        }
        #endregion
    }
    #endregion

    #region SRXAutoID
    public class SRXAutoID
    {
        public Image Img = new Bitmap(200, 200);
        xRingTCP SRX = new xRingTCP("AutoID", 2);
        xRingTCP SRImg = new xRingTCP("AutoImage", 5);
        string TrigOn = "LON\r", TrigOff = "LOFF\r";
        Task _doTrig = null, _doReadImage = null;
        bool inTrig = false, TempFlag = false;
        public int ImgCount = 0, RepllyCount = 0;
        byte[] SRBuffer = new byte[65536], JpegStart = new byte[] { 0xFF, 0xD8 };
        byte[] SrInit = new byte[] { 0x5B, 0x53, 0x4C, 0x50, 0x4B, 0x54, 0x21, 0x5D, 0xA4,
            0xAC, 0xB3, 0xAF, 0xB4, 0xAB, 0xDE, 0xA2, 0x00, 0x00, 0x00, 0x11, 0xC4, 0x00,
            0x01, 0x05, 0x02, 0x00, 0x00, 0x00, 0x00, 0x05, 0x4F, 0x03, 0xFF };
        byte[] SrStat = new byte[] { 0x5B, 0x53, 0x4C, 0x50, 0x4B, 0x54, 0x21, 0x5D, 0xA4,
            0xAC, 0xB3, 0xAF, 0xB4, 0xAB, 0xDE, 0xA2, 0x00, 0x00, 0x00, 0x06, 0x03, 0x02 };
        byte[] SrReq = new byte[] { 0x5B, 0x53, 0x4C, 0x50, 0x4B, 0x54, 0x21, 0x5D, 0xA4,
            0xAC, 0xB3, 0xAF, 0xB4, 0xAB, 0xDE, 0xA2, 0x00, 0x00, 0x00, 0x06, 0x03, 0x00 };
        byte[] SrReply = new byte[] { 0x5B, 0x53, 0x4C, 0x50, 0x4B, 0x54, 0x21, 0x5D, 0xA4,
            0xAC, 0xB3, 0xAF, 0xB4, 0xAB, 0xDE, 0xA2, 0x00, 0x00, 0x00, 0x06, 0x03, 0x81 };
        public bool Connected
        { get { return SRX.Connected; } }
        /// <summary>成功讀取返回 ID，否則返回 "" </summary>
        public Action<string> onReceive = null;
        public Action onImage = null;

        public void ConnectTo(string targetIP)
        {
            SRX.ConnectTo(targetIP, 9004, true);
            SRImg.ConnectTo(targetIP, 5920, true);
            if (_doReadImage == null)
                _doReadImage = Task.Factory.StartNew(doReadImage);
        }
        /// <summary>觸發讀取 ID</summary>
        public void Trig()
        {
            if (!Connected)
                return;
            if ((_doTrig == null) || _doTrig.IsCompleted)
                _doTrig = Task.Factory.StartNew(() =>
                {
                    string Temp = SRX.Receive();
                    SRX.Send(Encoding.ASCII.GetBytes(TrigOn), 0, TrigOn.Length, false);
                    SpinWait.SpinUntil(() => SRX.IndexOf(0x0D) > 0, 800);
                    bool success = SRX.IndexOf(0x0D) > 0;
                    if (onReceive != null)
                        onReceive(SRX.Receive(0x0D).Trim());
                    inTrig = true;
                    if (!success)
                    {
                        Thread.Sleep(500);
                        SRX.Send(Encoding.ASCII.GetBytes(TrigOff), 0, TrigOff.Length, false);
                    }
                });
        }

        int ReadResponse(ref byte[] Buf, bool Intr)
        {
            int Cnt = 0;
            SpinWait.SpinUntil(() => (SRImg.DataCount >= 20) || (Intr && inTrig), 4000);
            if (SRImg.DataCount < 20)
                return 0;
            SRImg.Receive(Buf, 0, 20);
            if (Encoding.ASCII.GetString(Buf, 0, 8) == "[SLPKT!]")
            {
                Array.Reverse(Buf, 16, 4);
                Cnt = BitConverter.ToInt32(Buf, 16) - 4;
                if (Buf.Length < Cnt)
                    Array.Resize(ref Buf, Cnt);
                SpinWait.SpinUntil(() => SRImg.DataCount >= Cnt, 1000);
                Cnt = SRImg.Receive(Buf, 0, Cnt);
                RepllyCount++;
            }
            else
                SRImg.Receive();
            return Cnt;
        }

        void doStatus(ref byte[] Buf)
        {
            Thread.Sleep(150);
            SRImg.Send(SrStat, 0, SrStat.Length, true);
            ReadResponse(ref SRBuffer, true);
        }

        void doReplyImage(ref byte[] Buf)
        {
            SRImg.Send(SrReq, 0, SrReq.Length, true);
            int Cnt = ReadResponse(ref SRBuffer, false);
            if (Cnt > 100)
            {
                int Index = SRImg.FindIndex(SRBuffer, JpegStart, 0, 100);
                if (Index >= 0)
                    try
                    {
                        ImgCount++;
                        MemoryStream st = new MemoryStream(SRBuffer, Index, Cnt - Index);
                        Image temp = Image.FromStream(st);
                        Img = temp;
                        if (onImage != null)
                            onImage();
                    }
                    catch(Exception ex)
                    { }
            }
            SRImg.Send(SrReply, 0, SrReply.Length, true);
            ReadResponse(ref SRBuffer, false);
            inTrig = false;
        }

        void doReadImage()
        {
            int S = 0, Cnt = 0;
            while (true)
            {
                switch (S)
                {
                    case 0:
                        if (SRImg.Connected)
                            S = 100;
                        break;
                    case 100:
                        SRImg.Send(SrInit, 0, SrInit.Length, true);
                        Cnt = ReadResponse(ref SRBuffer, false);
                        if (Cnt > 0)
                            S = 200;
                        else
                            Thread.Sleep(50);
                        break;
                    case 200:
                        if (SRImg.Connected)
                        {
                            if (!inTrig)
                                doStatus(ref SRBuffer);
                            else
                            {
                                SRImg.Send(SrInit, 0, SrInit.Length, true);
                                Cnt = ReadResponse(ref SRBuffer, false);
                                doReplyImage(ref SRBuffer);
                            }
                        }
                        else
                            S = 0;
                        break;
                }
                Thread.Sleep(5);
            }
        }
    }
    #endregion

    #region ModbusTCP
    public class ModbusTCP : IDisposable
    {
        xRingTCP target;
        bool disposed = false;
        byte _SlaveID = 0;
        ushort TrID = 0, ProtocolID = 0;

        public bool TimeOut(double baseTim, double range) { return (NowTime - baseTim) > range; }
        public double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        public bool Connected
        { get { return (target != null) && target.Connected; } }

        public ModbusTCP(string name)
        {
            target = new xRingTCP(name, 3);
        }

        public void ConnectTo(bool autoReconnect, byte SlaveID, string destIP, int destPort = 502)
        {
            _SlaveID = SlaveID;
            target.ConnectTo(destIP, destPort, autoReconnect);
        }

        void FillBaseData(ref byte[] Buf, byte fCode, ushort TotalDataCount, ushort startAddr, ushort count)
        {
            TrID++;
            SetWord(Buf, 0, TrID);
            SetWord(Buf, 2, ProtocolID);
            SetWord(Buf, 4, TotalDataCount);
            Buf[6] = _SlaveID;
            Buf[7] = fCode;
            SetWord(Buf, 8, startAddr);
            SetWord(Buf, 10, count);
        }
        /// <summary> 讀取 8 bytes 回應，有異常時讀取 9 bytes</summary>
        byte ReadStdResponse(ref byte[] Temp)
        {
            double now = NowTime;
            while ((target.DataCount < 9) && (!TimeOut(now, 1)))
                Thread.Sleep(5);
            //SpinWait.SpinUntil(() => target.DataCount > 8, 500);
            if (target.DataCount < 9)
            {
                target.Receive(Temp, 0, Temp.Length);
                return 0xFF;
            }
            target.Receive(Temp, 0, 8);
            if (Temp[7] > 127)
            {
                target.Receive(Temp, 0, 1);
                return Temp[0];
            }
            return 0;
        }
        /// <summary> 讀取Data</summary>
        byte ReadData(ref byte[] Temp, int size, int dropSize)
        {
            int TSize = size + dropSize;
            if (target.DataCount < TSize)
                SpinWait.SpinUntil(() => target.DataCount >= TSize, 500);
            if (Temp.Length < size)
                Array.Resize(ref Temp, size);
            if (dropSize > 0)
                target.Receive(Temp, 0, dropSize);
            int readCount = target.Receive(Temp, 0, size);
            return (readCount == size) ? (byte)0 : (byte)0xFE;
        }

        ushort GetWord(byte[] Temp, int index)
        {
            return (ushort)(Temp[index] * 256 + Temp[index + 1]);
        }

        void SetWord(byte[] Temp, int index, ushort Val)
        {
            Temp[index] = (byte)(Val / 256);
            Temp[index + 1] = (byte)(Val % 256);
        }

        void SwapWords(byte[] Temp)
        {
            for (int i = 0; i < Temp.Length; i += 2)
            {
                byte A1 = Temp[i];
                Temp[i] = Temp[i + 1];
                Temp[i + 1] = A1;
            }
        }

        void ExpandBool(byte[] Temp, int tempSize, bool[] Buf, int targetIndex, ushort count)
        {
            int BIndex = 0;
            int MaxCount = Math.Min(tempSize * 8, count);
            byte Val = 0;
            for (int i = 0; i < MaxCount; i++)
            {
                if ((i % 8) == 0)
                    Val = Temp[BIndex++];
                Buf[targetIndex++] = (Val & 1) != 0;
                Val >>= 1;
            }
        }

        bool[] ReadBit(byte FCode, ushort startAddr, out byte ErrCode, ushort count)
        {
            if (!target.Connected)
            {
                ErrCode = 0xFF;
                return null;
            }
            ErrCode = 0;
            byte[] Temp = new byte[12];
            FillBaseData(ref Temp, FCode, 6, startAddr, count);
            target.Send(Temp, 0, 12, true);
            ErrCode = ReadStdResponse(ref Temp);
            if (ErrCode != 0)
                return null;
            int size = GetWord(Temp, 4) - 2;
            ErrCode = ReadData(ref Temp, size, 1);
            if (ErrCode != 0)
                return null;
            int MaxCount = Math.Min(Temp[0] * 8, count);
            bool[] buf = new bool[MaxCount];
            int nIndex = 0;
            int nVal = 0;
            for (int i = 0; i < MaxCount; i++)
            {
                if ((i % 8) == 0)
                    nVal = Temp[nIndex++];
                bool Val = (nVal & 1) != 0;
                buf[i] = Val;
                nVal = (nVal >> 1);
            }
            return buf;
        }

        void ReadBit(byte FCode, ushort startAddr, out byte ErrCode, ushort count, bool[] Buf, int targetIndex)
        {
            ErrCode = 0;
            byte[] Temp = new byte[12];
            FillBaseData(ref Temp, FCode, 6, startAddr, count);
            target.Send(Temp, 0, 12, true);
            if ((ErrCode = ReadStdResponse(ref Temp)) == 0)
            {
                int size = GetWord(Temp, 4) - 2 - 1;
                if (ReadData(ref Temp, size, 1) == 0)
                    ExpandBool(Temp, size, Buf, targetIndex, count);
            }
        }

        void ExpandWord(byte[] Temp, int tempSize, byte[] Buf, int targetIndex)
        {
            for (int i = 0; i < tempSize; i += 2)
            {
                Buf[targetIndex++] = Temp[i + 1];
                Buf[targetIndex++] = Temp[i];
            }
        }

        byte[] ReadWord(byte FCode, ushort startAddr, out byte ErrCode, ushort count)
        {
            if ((count > 128) || (!target.Connected))
            {
                ErrCode = 0xFF;
                return null;
            }
            ErrCode = 0;
            byte[] Temp = new byte[12];
            FillBaseData(ref Temp, FCode, 6, startAddr, count);
            target.Send(Temp, 0, 12, true);
            ErrCode = ReadStdResponse(ref Temp);
            if (ErrCode != 0)
                return null;
            int size = GetWord(Temp, 4) - 2;
            ErrCode = ReadData(ref Temp, size - 1, 1);
            if (ErrCode != 0)
                return null;
            size--;
            byte[] buf = new byte[size];
            Array.Copy(Temp, 0, buf, 0, size);
            for (int i = 0; i < size; i += 2)
                Array.Reverse(buf, i, 2);
            return buf;
        }

        void ReadWord(byte FCode, ushort startAddr, out byte ErrCode, ushort count, byte[] Buf, int targetIndex)
        {
            ErrCode = 0;
            byte[] Temp = new byte[12];
            FillBaseData(ref Temp, FCode, 6, startAddr, count);
            target.Send(Temp, 0, 12, true);
            if ((ErrCode = ReadStdResponse(ref Temp)) == 0)
            {
                int size = GetWord(Temp, 4) - 2 - 1;
                if (ReadData(ref Temp, size, 1) == 0)
                    ExpandWord(Temp, size, Buf, targetIndex);
            }
        }
        /// <summary> 讀取 0XXXX DO 值，返回異常碼</summary>
        public byte Read_DO(ushort startAddr, ushort count, bool[] Buf, int targetIndex)
        {
            byte ErrCode;
            ReadBit(1, startAddr, out ErrCode, count, Buf, targetIndex);
            return ErrCode;
        }
        /// <summary> 寫入 0XXXX DO 值，返回異常碼</summary>
        public void Write_DO(bool[] Buf, int Index, ushort startAddr, out byte ErrCode, ushort count)
        {
            ErrCode = 0;
            int TotalData = count / 8;
            if ((count % 8) != 0) TotalData++;
            if (TotalData > 255)
                ErrCode = 3;
            else
            {
                byte[] Temp = new byte[13 + TotalData];
                FillBaseData(ref Temp, 15, (ushort)(TotalData + 7), startAddr, count);
                Temp[12] = (byte)TotalData;
                byte mask = 1;
                for (int i = 0; i < count; i++)
                {
                    int target = i / 8;
                    byte nMask = (byte)(255 - mask);
                    if (Buf[Index + i])
                        Temp[13 + target] |= mask;
                    else
                        Temp[13 + target] &= nMask;
                    mask *= 2;
                    if ((i % 8) == 7)
                        mask = 1;
                }
                target.Send(Temp, 0, Temp.Length, true);
                ErrCode = ReadStdResponse(ref Temp);
                if (ErrCode != 0)
                    return;
                int size = GetWord(Temp, 4) - 2;
                ErrCode = ReadData(ref Temp, size, 0);
            }
        }
        /// <summary> 讀取 1XXXX DI 值，返回異常碼</summary>
        public byte Read_DI(ushort startAddr, ushort count, bool[] Buf, int targetIndex)
        {
            byte ErrCode;
            ReadBit(2, startAddr, out ErrCode, count, Buf, targetIndex);
            return ErrCode;
        }
        /// <summary> 讀取 3XXXX AI 值，返回異常碼</summary>
        public byte Read_AI(ushort startAddr, ushort count, byte[] Buf, int targetIndex)
        {
            byte ErrCode;
            ReadWord(4, startAddr, out ErrCode, count, Buf, targetIndex);
            return ErrCode;
            //return ReadWord(4, startAddr, out ErrCode, count);
        }
        /// <summary> 讀取 4XXXX AO 值，返回異常碼</summary>
        public byte Read_AO(ushort startAddr, ushort count, byte[] Buf, int targetIndex)
        {
            byte ErrCode;
            ReadWord(3, startAddr, out ErrCode, count, Buf, targetIndex);
            return ErrCode;
            //return ReadWord(3, startAddr, out ErrCode, count);
        }
        /// <summary> 寫入 4XXXX AO 值，返回異常碼</summary>
        public void Write_AO(ushort[] Buf, int Index, ushort startAddr, out byte ErrCode, ushort count)
        {
            ErrCode = 0;
            int TotalData = count * 2;
            if (TotalData > 255)
                ErrCode = 3;
            else
            {
                byte[] Temp = new byte[13 + TotalData];
                FillBaseData(ref Temp, 16, (ushort)(TotalData + 7), startAddr, count);
                Temp[12] = (byte)TotalData;
                for (int i = 0; i < count; i++)
                {
                    Temp[13 + i * 2] = (byte)(Buf[Index + i] / 256);
                    Temp[13 + i * 2 + 1] = (byte)(Buf[Index + i] % 256);
                }
                target.Send(Temp, 0, Temp.Length, true);
                ErrCode = ReadStdResponse(ref Temp);
                if (ErrCode != 0)
                    return;
                int size = GetWord(Temp, 4) - 2;
                ErrCode = ReadData(ref Temp, size, 0);
            }
        }

        public void Close()
        {
            target.Close();
        }

        #region IDisposable實作
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ModbusTCP() { Dispose(false); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Close();
            disposed = true;
        }
        #endregion

    }
    #endregion

    #region MEWTOCOL
    public class MEWTOCOL : IDisposable
    {
        xRingTCP target;
        bool disposed = false;
        byte _SlaveID = 0;
        string SlaveStr = "00";

        public MEWTOCOL(string name)
        {
            target = new xRingTCP(name, 3);
        }

        public void ConnectTo(bool autoReconnect, byte SlaveID, string destIP, int destPort = 32769)
        {
            _SlaveID = SlaveID;
            SlaveStr = string.Format("{0:00}", SlaveID);
            target.ConnectTo(destIP, destPort, autoReconnect);
        }

        public void Close()
        {
            target.Close();
        }

        int ReadBit(string PreFix, int startNo, int EndNo, bool[] Buf, int targetIndex)
        {
            string Cmd = string.Format("<{0}#RCC{1}", SlaveStr, PreFix);
            return 0;
        }

        public int ReadBitX(int startNo, int EndNo, bool[] Buf, int targetIndex)
        { return ReadBit("X", startNo, EndNo, Buf, targetIndex); }

        public int ReadBitY(int startNo, int EndNo, bool[] Buf, int targetIndex)
        { return ReadBit("Y", startNo, EndNo, Buf, targetIndex); }

        public int ReadBitR(int startNo, int EndNo, bool[] Buf, int targetIndex)
        { return ReadBit("R", startNo, EndNo, Buf, targetIndex); }

        public int ReadBitL(int startNo, int EndNo, bool[] Buf, int targetIndex)
        { return ReadBit("L", startNo, EndNo, Buf, targetIndex); }

        public int ReadBitT(int startNo, int EndNo, bool[] Buf, int targetIndex)
        { return ReadBit("T", startNo, EndNo, Buf, targetIndex); }

        public int ReadBitC(int startNo, int EndNo, bool[] Buf, int targetIndex)
        { return ReadBit("C", startNo, EndNo, Buf, targetIndex); }

        public string AddBCC(string command)
        {
            byte[] temp = Encoding.Default.GetBytes(command);
            byte V = 0;
            foreach (var P in temp)
                V ^= P;
            return command + string.Format("{0:X2}\n", V);
        }

        #region IDisposable實作
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MEWTOCOL() { Dispose(false); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Close();
            disposed = true;
        }
        #endregion

    }
    #endregion

    #region Danikor
    public class Danikor : IDisposable
    {
        public enum ActionType { None, DoRecipe, CommandOK, CommandNG, Finish }
        int Step = 0;
        bool disposed = false, ExitP = false;
        xRingTCP serv;
        Task _doWork;
        public Action OnFinish;
        object dSend = new object(), dRecv = new object();
        ActionType _LoadRun, _Light, _LoadRecipe, _Silo, _料倉, _aSilo;
        public bool Connected
        {
            get { return serv.Clients.Count > 0; }
        }

        public Danikor()
        {
            serv = new xRingTCP("Danikor", 5);
            Application.ApplicationExit += onExit;
        }

        public void Start(string LocalIP, int LocalPort = 6032)
        {
            ExitP = false;
            serv.Listen(LocalIP, LocalPort, 2);
            _doWork = Task.Factory.StartNew(CheckConnection);
        }

        void CheckConnection()
        {
            while (!ExitP)
            {
                while ((!ExitP) && (serv.Clients.Count == 0))
                    Thread.Sleep(50);
                Step = 0;
                while ((!ExitP) && (serv.Clients.Count > 0))
                {
                    var target = serv.Clients.FirstOrDefault();
                    if (target != null)
                        Scan(target, ref Step);
                    Thread.Sleep(5);
                }
            }
        }

        void CheckActionResult(string[] split, string cmd, ref ActionType _state)
        {
            if ((split[2] == cmd) && (split.Length > 4))
            {
                if (split[4] == "0")
                    _state = ActionType.CommandNG;
                if (split[4] == "1")
                    _state = ActionType.CommandOK;
                if (split[4] == "3")
                    _state = ActionType.Finish;
            }
        }

        void Scan(xRingTCP target, ref int S)
        {
            int index = target.IndexOf(10);
            if (index > 0)
            {
                string Msg = target.Receive(10).Trim();
                string[] split = Msg.Split(',');
                if (split.Length > 4)
                {
                    if ((split[2] == "32") || (split[2] == "33"))
                        _Silo = ActionType.CommandOK;
                    CheckActionResult(split, "40", ref _Light);
                    CheckActionResult(split, "36", ref _LoadRun);
                    CheckActionResult(split, "47", ref _LoadRecipe);
                    CheckActionResult(split, "41", ref _料倉);
                    CheckActionResult(split, "37", ref _aSilo);
                }
            }
        }
        /// <summary>執行配方，回傳已執行完成(true) / 失敗</summary>
        public bool LoadtoRun(int RecipeNo)
        {
            if (!Connected)
                return false;
            bool success = false;
            _LoadRun = ActionType.None;
            string cmd = string.Format("AB,1,36,1,{0},\r\n", RecipeNo);
            var target = serv.Clients.First();
            target.Send(Encoding.Default.GetBytes(cmd), 0, cmd.Length, false);
            while (Connected && (_LoadRun == ActionType.None))
                Thread.Sleep(5);
            if (_LoadRun != ActionType.CommandOK)
                return false;
            while (Connected && (_LoadRun != ActionType.Finish))
                Thread.Sleep(5);
            success = Connected;
            _LoadRun = ActionType.None;
            return success;
        }
        /// <summary>開關燈</summary>
        public bool Light(int brightness)
        {
            if (!Connected)
                return false;
            _Light = ActionType.None;
            string cmd = string.Format("AB,1,40,1,{0},\r\n", Math.Min(brightness, 60));
            var target = serv.Clients.First();
            target.Send(Encoding.Default.GetBytes(cmd), 0, cmd.Length, false);
            while (Connected && (_Light == ActionType.None))
                Thread.Sleep(5);
            bool success = _Light == ActionType.CommandOK;
            _Light = ActionType.None;
            return success;
        }
        /// <summary>
        /// 柔性+料倉震動，需先載入配方
        /// </summary>
        /// <param name="StartPhase1">開始相位</param>
        /// <param name="EndPhase1">結束相位</param>
        /// <param name="StartPhase2">開始相位</param>
        /// <param name="EndPhase2">結束相位</param>
        /// <param name="StartPhase3">開始相位</param>
        /// <param name="EndPhase3">結束相位</param>
        /// <param name="StartPhase4">開始相位</param>
        /// <param name="EndPhase4">結束相位</param>
        /// <param name="Watt">功率( 0 - 100%)</param>
        /// <param name="SiloPhase">料倉結束相位</param>
        /// <param name="FreqHz">頻率( 0 - 150Hz)</param>
        /// <param name="Voltage">料倉震福( 0 - 100%)</param>
        /// <returns></returns>
        public bool Silo(double StartPhase1, double EndPhase1, double StartPhase2, double EndPhase2, double StartPhase3, double EndPhase3, double StartPhase4, double EndPhase4, double Watt, int SiloPhase, double FreqHz, double Voltage)
        {
            if (!Connected)
                return false;
            _Silo = ActionType.None;
            Watt = Math.Min(Math.Max(Watt, 0), 100);
            FreqHz = Math.Max(FreqHz, 0) * 10;
            Voltage = Math.Min(Math.Max(Voltage, 0), 100);
            SiloPhase = Math.Min(Math.Max(SiloPhase, 0), 180);
            string cmd = "";
            if ((Voltage > 0.1) || (Watt > 0.1))
                cmd = string.Format("AB,1,32,16,{0:F0},{3:F0},{4:F0},{5:F0},{6:F0},{7:F0},{8:F0},{9:F0},{10:F0},0,{1},{11:F0},{11:F0},{11:F0},{11:F0},{2:F0},\r\n",
                    FreqHz, SiloPhase, Voltage * 10,
                    StartPhase1 * 10, EndPhase1 * 10, StartPhase2 * 10, EndPhase2 * 10,
                    StartPhase3 * 10, EndPhase3 * 10, StartPhase4 * 10, EndPhase4 * 10,
                    Watt * 10);
            else
                cmd = "AB,1,33,1,0,\r\n";
            var target = serv.Clients.First();
            target.Send(Encoding.Default.GetBytes(cmd), 0, cmd.Length, false);
            while (Connected && (_Silo == ActionType.None))
                Thread.Sleep(5);
            return Connected;
        }

        /// <summary>入料料倉震動，需先載入配方</summary>
        public bool Silo(int SiloPhase, double FreqHz, int Voltage)
        {
            if (!Connected)
                return false;
            _Silo = ActionType.None;
            FreqHz = Math.Max(FreqHz, 0) * 10;
            Voltage = Math.Min(Math.Max(Voltage, 0), 90) * 10;
            SiloPhase = Math.Min(Math.Max(SiloPhase, 0), 180);
            string cmd = "";
            //string cmd = string.Format("AB,1,32,16,{0:F0},0,0,0,0,0,0,0,0,0,{1},0,0,0,0,{2},\r\n", FreqHz, Phase, Voltage);
            if (Voltage != 0)
                cmd = string.Format("AB,1,32,16,{0:F0},0,500,500,1000,0,500,500,1000,0,{1},60,60,60,60,{2},\r\n", FreqHz, SiloPhase, Voltage);
            else
                cmd = "AB,1,33,1,0,\r\n";
            var target = serv.Clients.First();
            target.Send(Encoding.Default.GetBytes(cmd), 0, cmd.Length, false);
            while (Connected && (_Silo == ActionType.None))
                Thread.Sleep(5);
            _Silo = ActionType.None;
            return Connected;
        }
        /// <summary>載入配方</summary>
        public bool LoadRecipe(int No)
        {
            if (!Connected)
                return false;
            _LoadRecipe = ActionType.None;
            string cmd = string.Format("AB,1,47,1,{0},\r\n", No);
            var target = serv.Clients.First();
            target.Send(Encoding.Default.GetBytes(cmd), 0, cmd.Length, false);
            while (Connected && (_LoadRecipe == ActionType.None))
                Thread.Sleep(5);
            var success = _LoadRecipe == ActionType.CommandOK;
            _LoadRecipe = ActionType.None;
            return success;
        }

        public bool ActiveSilo(int RecipeNo, bool OnOff)
        {
            if (!Connected)
                return false;
            _aSilo = ActionType.None;
            int aOn = (OnOff) ? 1 : 0;
            string cmd = string.Format("AB,1,37,5,{0},{1},2,2,2,\r\n", RecipeNo, aOn);
            var target = serv.Clients.First();
            target.Send(Encoding.Default.GetBytes(cmd), 0, cmd.Length, false);
            while (Connected && (_aSilo == ActionType.None))
                Thread.Sleep(5);
            var success = _aSilo == ActionType.CommandOK;
            _aSilo = ActionType.None;
            return success;
        }

        void onExit(object sender, EventArgs e)
        {
            ExitP = true;
            Close();
        }

        public void Close()
        {
            if (serv.Connected)
                Light(0);
            ExitP = true;
            serv.Close();
        }

        #region IDisposable實作
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Danikor() { Dispose(false); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Close();
            disposed = true;
        }
        #endregion

    }
    #endregion

    #region OPTControler 一般光源
    public class OPTControlerRS : IDisposable
    {
        bool disposed = false, ExitP = false;
        SerialPort serv;
        byte[] inBuf = new byte[32];
        bool isOpen = false;
        Task _work;
        public bool Connected = false;
        public int[] Lights;
        int[] PreLights;

        public bool TimeOut(double baseTim, double range) { return (NowTime - baseTim) > range; }
        public double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }


        public OPTControlerRS(int MaxLight)
        {
            Lights = new int[MaxLight];
            PreLights = new int[MaxLight];
            Application.ApplicationExit += onExit;
        }

        void onExit(object sender, EventArgs e)
        {
            ExitP = true;
            Close();
        }

        public void Open(string ComPort)
        {
            ExitP = false;
            try
            {
                serv = new SerialPort(ComPort, 9600, Parity.None, 8);
                serv.Encoding = Encoding.Default;
                serv.ReadTimeout = 1000;
                serv.Open();
                isOpen = true;
            }
            catch (Exception ex)
            { isOpen = false; }
            if (isOpen)
                if ((_work == null) || _work.IsCompleted || _work.IsFaulted || _work.IsCanceled)
                    _work = Task.Factory.StartNew(Scan);
        }

        public void Close()
        {
            ExitP = true;
            serv.Close();
            Connected = false;
            isOpen = false;
        }

        void SendCommand(byte[] Cmd)
        {
            byte temp = 0;
            for (int i = 0; i < Cmd.Length; i++)
                temp ^= Cmd[i];
            byte[] outBuf = new byte[Cmd.Length + 1];
            Array.Copy(Cmd, 0, outBuf, 0, Cmd.Length);
            outBuf[outBuf.Length - 1] = temp;
            try
            {
                serv.Write(outBuf, 0, outBuf.Length);
            }
            catch { }
        }

        bool ReadIntensity(int Channel, ref int Val)
        {
            SendCommand(new byte[] { 0xFF, 0x11, (byte)Channel, 0, 0 });
            double now = NowTime;
            bool success = false;
            try
            {
                while ((serv.BytesToRead < 6) && (!TimeOut(now, 1)))
                    Thread.Sleep(5);
                int Cnt = serv.Read(inBuf, 0, 6);
                success = (Cnt == 6);
                if (Cnt == 6)
                {
                    Array.Reverse(inBuf, 3, 2);
                    Val = BitConverter.ToInt16(inBuf, 3);
                }
            }
            catch { }
            return success;
        }

        bool SetIntensity(int Channel, int Val)
        {
            byte[] temp = new byte[] { 0xFF, 0x01, (byte)Channel, 0, 0 };
            byte[] bVal = BitConverter.GetBytes(Val);
            Array.Reverse(bVal, 0, 2);
            Array.Copy(bVal, 0, temp, 3, 2);
            SendCommand(temp);
            double now = NowTime;
            while ((serv.BytesToRead < 2) && (!TimeOut(now, 1)))
                Thread.Sleep(5);
            bool success = false;
            try
            {
                int Cnt = serv.Read(inBuf, 0, 2);
                success = (Cnt == 2);
            }
            catch { }
            return success;
        }

        void ReadIntensity()
        {
            bool success = true;
            for (int i = 0; i < Lights.Length; i++)
            {
                int temp = 0;
                if (ReadIntensity(i + 1, ref temp))
                {
                    Lights[i] = temp;
                    PreLights[i] = temp;
                }
                else
                    success = false;
            }
            Connected = success;
        }

        void ScanLight()
        {
            for (int i = 0; i < Lights.Length; i++)
            {
                if (Lights[i] != PreLights[i])
                {
                    //if (Lights[i] == 0)
                    //    Connected = serv.TurnOffChannel(i) == 0;
                    //if (PreLights[i] == 0)
                    //    Connected = serv.TurnOnChannel(i) == 0;
                    Connected = SetIntensity(i, Lights[i]);
                    PreLights[i] = Lights[i];
                }
            }
        }

        void Scan()
        {
            while (isOpen && (!ExitP))
            {
                if (!Connected)
                    ReadIntensity();
                else
                    ScanLight();
                Thread.Sleep(10);
            }
        }

        #region IDisposable實作
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~OPTControlerRS() { Dispose(false); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Close();
            disposed = true;
        }
        #endregion

    }
    #endregion

    #region AutoErase
    public static class AutoErase
    {
        static Task nTask;
        static object LockChange = new object();
        static List<Tuple<string, int>> nDir = new List<Tuple<string, int>>();
        public static bool TimeOut(double baseTim, double range) { return (NowTime - baseTim) > range; }
        public static double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        public static void Add(string DirFullName, int LimitDays)
        {
            if (nTask == null)
                nTask = Task.Factory.StartNew(DoSearch);
            lock (LockChange)
                nDir.Add(new Tuple<string, int>(DirFullName, LimitDays));
        }
        /// <summary>每小時搜尋一次</summary>
        static void DoSearch()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Thread.Sleep(15000);
            Search();
            while (true)
            {
                Search();
                double nTime = NowTime;
                while (!TimeOut(nTime, 60 * 60))
                    Thread.Sleep(1000);
            }
        }
        static void DoErase(DateTime nowdate, Tuple<string, int> targetPara, string targetPath)
        {
            int Between = Math.Abs(new TimeSpan(DateTime.Now.Ticks - nowdate.Ticks).Days);
            if (Between > targetPara.Item2)
                try
                {
                    Debug.Print("AutoRease {0}", targetPath);
                    Directory.Delete(targetPath, true);
                }
                catch { }
        }
        static void Search()
        {
            try
            {
                for (int i = nDir.Count - 1; i >= 0; i--)
                    if (Directory.Exists(nDir[i].Item1))
                    {
                        var Dirs = Directory.EnumerateDirectories(nDir[i].Item1, "*.*", SearchOption.TopDirectoryOnly).ToArray();
                        foreach (var P in Dirs)
                        {
                            DateTime nowdate;
                            string date = Path.GetFileName(P);
                            if (DateTime.TryParseExact(date, "yyyyMMdd", null, DateTimeStyles.None, out nowdate))
                                DoErase(nowdate, nDir[i], P);
                            else
                            {
                                nowdate = File.GetCreationTime(P);
                                DoErase(nowdate, nDir[i], P);
                            }
                            Thread.Sleep(2000);
                        }
                    }
            }
            catch
            { }
        }
    }
    #endregion

    #region Exlite
    public class Exlite : IDisposable
    {
        SerialPort serv;
        byte[] inBuf = new byte[32];
        bool ExitP = false;
        public bool Connected = false, isOpen = false;
        byte ID = 1;
        Task _work;
        public int[] Lights;
        int[] PreLights;

        public bool TimeOut(double baseTim, double range) { return (NowTime - baseTim) > range; }
        public double NowTime
        {
            get { return DateTime.Now.Ticks / 10000000.0; }
        }

        public Exlite(int MaxLight)
        {
            Lights = new int[MaxLight];
            PreLights = new int[MaxLight];
            Application.ApplicationExit += onExit;
        }

        void onExit(object sender, EventArgs e)
        {
            ExitP = true;
            Close();
        }

        public void Open(string ComPort, byte controlerID)
        {
            ExitP = false;
            try
            {
                ID = controlerID;
                serv = new SerialPort(ComPort, 115200, Parity.None, 8);
                serv.Encoding = Encoding.Default;
                serv.ReadTimeout = 1000;
                serv.Open();
                isOpen = true;
                Connected = true;
            }
            catch (Exception ex)
            { isOpen = false; }
            if (isOpen)
                if ((_work == null) || _work.IsCompleted || _work.IsFaulted || _work.IsCanceled)
                    _work = Task.Factory.StartNew(Scan);
        }

        #region CRC計算
        public ushort CalcCRC(byte[] nData, int Count)
        {
            ushort CRC = 0xffff;
            for (int i = 0; i < Count; i++)
            {
                CRC ^= nData[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((CRC % 2) != 0)
                        CRC = (ushort)((CRC / 2) ^ 0xa001);
                    else
                        CRC = (ushort)(CRC / 2);
                }
            }
            return CRC;
        }
        #endregion

        void ReadIntensity(ref byte[] Buf)
        {
            for (int i = 0; i < Lights.Length; i++)
            {
                int startAddr = ((i + 1) << 12) + 10;
                Connected = ReadAO(startAddr, 1, ref Buf) != 255;
                if (Connected)
                    Lights[i] = PreLights[i] = BitConverter.ToInt16(Buf, 0);
            }
        }

        void ScanLight()
        {
            for (int i = 0; i < Lights.Length; i++)
            {
                if (isOpen && (Lights[i] != PreLights[i]))
                {
                    int startAddr = ((i + 1) << 12) + 10;
                    Connected = WriteAO(startAddr, (ushort)(Lights[i] % 65536)) != 255;
                    PreLights[i] = Lights[i];
                }
            }
        }

        public byte ReadOneRegister(int RegisterAddr, ref int Val)
        {
            byte ErrCode = 255;
            if (!Connected)
            {
                int ReadCount = 1;
                byte[] dVal = BitConverter.GetBytes(RegisterAddr);
                byte[] dCount = BitConverter.GetBytes(ReadCount);
                byte[] temp = new byte[] { ID, 3, dVal[1], dVal[0], dCount[1], dCount[0], 0, 0 };
                BitConverter.GetBytes(CalcCRC(temp, temp.Length - 2)).CopyTo(temp, temp.Length - 2);
                try
                {
                    serv.Write(temp, 0, temp.Length);
                    byte[] Buf = new byte[ReadCount * 2];
                    ErrCode = ResponseWithDataWord(ref Buf);
                    if (ErrCode == 0)
                        Val = BitConverter.ToInt16(Buf, 0);
                    //ErrCode = ReadResponseOneReg(ref Val);
                }
                catch
                { ErrCode = 255; }
            }
            return ErrCode;
        }

        //Normal ID, Cmd, byteCount, Data1,..Datan, CRC0, CRC1
        //Error ID, ErrCmd, ErrCode, CRC0, CRC1
        byte ResponseWithDataWord(ref byte[] outData)
        {
            byte ErrCode = 255;
            byte[] Buf = new byte[3];
            int Cnt = serv.Read(Buf, 0, 3);
            if (Cnt == 3)
            {
                int Req = (Buf[1] < 128) ? Buf[2] : 0;
                if ((outData == null) || (outData.Length < Req))
                    Array.Resize(ref outData, Req);
                //ReadData
                if (Req > 0)
                {
                    Cnt = serv.Read(outData, 0, Req);
                    for (int i = 0; i < Cnt; i += 2)
                        Array.Reverse(outData, i, 2);
                }
                else
                    ErrCode = Buf[2];
                //ReadCRC
                serv.Read(Buf, 0, 2);
            }
            return ErrCode;
        }

        byte ResponseNoData()
        {
            byte ErrCode = 255;
            byte[] Buf = new byte[6];
            int Cnt = serv.Read(Buf, 0, 3);
            if (Cnt == 3)
            {
                int Req = (Buf[1] < 128) ? 1 : 0;
                //ReadData
                if (Req > 0)
                {
                    serv.Read(Buf, 3, Req);
                    ErrCode = 0;
                }
                else
                    ErrCode = Buf[2];
                //ReadCRC
                serv.Read(Buf, 4, 2);
            }
            return ErrCode;
        }

        byte ReadAO(int startAddr, int Count, ref byte[] outData)
        {
            byte ErrCode = 255;
            if (isOpen)
            {
                byte[] dAddr = BitConverter.GetBytes(startAddr);
                byte[] dCount = BitConverter.GetBytes(Count);
                byte[] temp = new byte[] { ID, 3, dAddr[1], dAddr[0], dCount[1], dCount[0], 0, 0 };
                BitConverter.GetBytes(CalcCRC(temp, temp.Length - 2)).CopyTo(temp, temp.Length - 2);
                try
                {
                    serv.Write(temp, 0, temp.Length);
                    ErrCode = ResponseWithDataWord(ref outData);
                }
                catch
                { ErrCode = 255; }
            }
            return ErrCode;
        }

        byte WriteAO(int startAddr, ushort Val)
        {
            byte ErrCode = 0;
            if (isOpen)
            {
                byte[] dAddr = BitConverter.GetBytes(startAddr);
                byte[] dVal = BitConverter.GetBytes(Val);
                byte[] temp = new byte[] { ID, 6, dAddr[1], dAddr[0], dVal[1], dVal[0], 0, 0 };
                BitConverter.GetBytes(CalcCRC(temp, temp.Length - 2)).CopyTo(temp, temp.Length - 2);
                try
                {
                    serv.Write(temp, 0, temp.Length);
                    serv.ReadExisting();
                    //ErrCode = ResponseNoData();
                }
                catch (Exception ex)
                { ErrCode = 255; }
            }
            return ErrCode;
        }

        //byte ReadResponseOneReg(ref int Val)
        //{
        //    byte ErrCode = 255;
        //    double tim = NowTime;
        //    while ((serv.BytesToRead < 2) && (!TimeOut(tim, 1)))
        //        Thread.Sleep(50);
        //    byte[] Buf = new byte[8];
        //    if (serv.BytesToRead > 1)
        //    {
        //        serv.Read(Buf, 0, 2);
        //        int Req = (Buf[1] < 128) ? 6 : 3;
        //        tim = NowTime;
        //        while ((serv.BytesToRead < Req) && (!TimeOut(tim, 1)))
        //            Thread.Sleep(50);
        //        int Cnt = Math.Min(Req, serv.BytesToRead);
        //        if (Cnt > 0)
        //        {
        //            serv.Read(Buf, 0, Cnt);
        //            if (Cnt == Req)
        //            {
        //                ErrCode = (Buf[1] < 128) ? (byte)0 : Buf[2];
        //                Array.Reverse(Buf, 4, 2);
        //                if (ErrCode == 0)
        //                    Val = BitConverter.ToUInt16(Buf, 4);
        //            }
        //        }
        //    }
        //    return ErrCode;
        //}

        void Scan()
        {
            while (isOpen && (!ExitP))
            {
                //if (!Connected)
                //    ReadIntensity(ref inBuf);
                //else
                ScanLight();
                Thread.Sleep(10);
            }

        }

        public void Close()
        {
            isOpen = false;
            _work?.Wait();
            serv?.Close();
        }

        #region IDisposable實作
        public bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Exlite() { Dispose(false); }
        protected void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Close();
            disposed = true;
        }
        #endregion

    }
    #endregion

}
