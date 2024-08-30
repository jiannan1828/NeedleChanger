import struct
import logging
import logging.handlers
import ctypes
from sys import platform

if platform == "win32":
    OutputDebugString = ctypes.windll.kernel32.OutputDebugStringW
    
    class DbgViewHandler(logging.Handler):
        def emit(self, record):
            OutputDebugString(self.format(record))

def create_log(name, level, filename):
    # 1、创建日志收集器
    log = logging.getLogger(name)

    # 2、创建日志收集器的等级
    log.setLevel(level)

    # 3、设置日志的输出格式
    formats = "%(asctime)s - [%(filename)s-->%(funcName)s@%(lineno)d] - %(levelname)s:%(message)s"
    log_format = logging.Formatter(fmt=formats)
    
    # 4、创建日志收集渠道和等级  
    if platform == "win32":
        odh = DbgViewHandler()
        odh.setLevel(level)
        odh.setFormatter(log_format)
        log.addHandler(odh)
    elif platform == "linux" or platform == "linux2":
        fh = logging.handlers.RotatingFileHandler(filename=filename, encoding="utf-8", mode="w", maxBytes=500, backupCount=3)
        fh.setLevel(level)
        fh.setFormatter(log_format)
        log.addHandler(fh)
    
    log.info(msg="--------start--------")
    log.error(msg="--------start--------")
    return log


if platform == "linux" or platform == "linux2":
    filename = "/root/vmlogs/log/GateWay/Modules.log"
elif platform == "win32":
    filename = ""
    
log = create_log(name=__name__, level=logging.ERROR, filename=filename)
    
def HandShake(paramlist):#开始通信命令CR
    Register = "CR" + "\r"

    return Register

def HandShakeParase(paramlist,RecvData):#开始通信CR的响应CC
    if(RecvData== b"CC\r\n"):
        return "ok"
    else:
        return ""
    
def WriteArrayAssembly(paramlist,datalist):
    #log.error("keyence: WriteArrayAssembly start")
    package = ""
    data = ""
    nWriteNum = 0

    if len(paramlist) < 7:
        return package
    OrderType = int(paramlist[0])
    IntDataType = int(paramlist[1])
    SoftElementType = int(paramlist[2])
    SoftElementAddress = int(paramlist[3])
    SoftElementNum = int(paramlist[4])

    package += "WRS "
    if SoftElementType == 1:  # 软元件D
        package += "DM"
        if IntDataType == 3:
            return ""
    elif SoftElementType == 3:  # 软元件W
        package += "W"
        if IntDataType == 3:
            return ""
    elif SoftElementType == 4:  # 软元件R
        package += "R"
        if IntDataType != 3:
            return ""
    elif SoftElementType == 2:  # 软元件MR
        package += "MR"
        if IntDataType != 3:
            return ""
    elif SoftElementType == 5:  # 软元件EM
        package += "EM"
        if IntDataType == 3:
            return ""
    package += str(SoftElementAddress)

    for value in datalist:
        if int == type(value):
            if IntDataType == 1:  # 16位
                if value > 65535 or value < -32768:
                    package = ""
                    return package
                data += ' ' + str(int(value) & 0xffff)
                nWriteNum += 1
            elif IntDataType == 2:  # 32位，需要两个寄存器
                byte = struct.pack('i', value)
                nTemp1 = (byte[0] & 0x000000ff) | ((byte[1] & 0x000000ff) << 8)
                nTemp2 = (byte[2] & 0x000000ff) | ((byte[3] & 0x000000ff) << 8)
                data += " " + str(nTemp1) + " " + str(nTemp2)
                nWriteNum += 2
            elif IntDataType == 3:
                if 0 == int(value):
                    data += ' 0'
                else:
                    data += ' 1'
                nWriteNum += 1
            elif IntDataType == 4:
                if 0 == int(value):
                    data += ' 0'
                else:
                    data += ' 1'
                nWriteNum += 1
        elif float == type(value):
            if IntDataType == 3:
                return ""
            #log.error("keyence: float")
            byte = struct.pack('f', value)
            nTemp1 = (byte[0] & 0x000000ff) | ((byte[1] & 0x000000ff) << 8)
            nTemp2 = (byte[2] & 0x000000ff) | ((byte[3] & 0x000000ff) << 8)
            data += " " + str(nTemp1) + " " + str(nTemp2)
            nWriteNum += 2

        elif str == type(value):
            if IntDataType == 3:
                return ""
            #log.error("keyence: string")
            # for index in range(len(value)):
            #     temp = ord(value[index])
            #     data += ' ' + str(temp)
            #     nWriteNum += 1
            i = 0
            while( i < len(value)):
                #log.error("keyence: i = %d", i)
                temp1 = ord(value[i:i+1])
                #log.error("keyence: temp1 = %d", temp1)
                i += 1
                if i < len(value):
                    temp2 = ord(value[i:i+1])
                else:
                    temp2 = 0
                #log.error("keyence: temp2 = %d", temp2)
                temp = temp1 | (temp2 << 8)
                #log.error("keyence: temp = %d", temp)
                data += ' ' + str(temp)
                i += 1
                nWriteNum += 1

        elif bytes == type(value):
            if IntDataType == 3:
                return ""
            #log.error("keyence: bytes")
            i = 0
            while (i < len(value)):
                #log.error("keyence: i = %d", i)
                temp1 = value[i]
                #log.error("keyence: temp1 = %d", temp1)
                i += 1
                if i < len(value):
                    temp2 = value[i]
                else:
                    temp2 = 0
                #log.error("keyence: temp2 = %d", temp2)
                temp = temp1 | (temp2 << 8)
                #log.error("keyence: temp = %d", temp)
                data += ' ' + str(temp)
                i += 1
                nWriteNum += 1


    if nWriteNum > SoftElementNum:
            package = ""
            return package

    package = package + " " + str(nWriteNum) + data


    package += "\r"
    #log.error("keyence: package = %s", package)
    return package

def ReadAssembly(paramlist):
    package = ""

    if len(paramlist) < 7:
        return package
    OrderType = int(paramlist[0])
    IntDataType = int(paramlist[1])
    SoftElementType = int(paramlist[2])
    SoftElementAddress = int(paramlist[3])
    SoftElementNum = int(paramlist[4])

    package = "RDS "
    if SoftElementType == 1:  # 软元件D
        package += "DM"
    elif SoftElementType == 3:  # 软元件W
        package += "W"
        if IntDataType == 3:
            return ""
    elif SoftElementType == 2:  # 软元件R
        package += "MR"
    elif SoftElementType == 4:  # 软元件R
        package += "R"
    elif SoftElementType == 5:  # 软元件EM
        package += "EM"
    package += str(SoftElementAddress) + " " + str(SoftElementNum) + "\r"

    #log.error("keyence: %s", package)
    return package

def WritePrase(paramlist,RecvData):
    #log.error("keyence: %s", RecvData)
    if(RecvData== b"OK\r\n"):
        return "ok"
    else:
        return ""

def ReadPrase(paramlist,RecvData):
    #log.error("keyence: %s", RecvData)
    if len(paramlist) < 7:
        return ""
    strHexRecvEnable = paramlist[5]
    if (RecvData == b"E0\r\n") or (RecvData == b"E1\r\n"):
        return ""
    else:
        #log.error("keyence: before package")
        package = RecvData[0: -2]
        #log.error("keyence: after package")
        if strHexRecvEnable == 'true':
            strlist = package.split()
            package = b''
            for info in strlist:
                package += int(info).to_bytes(2, byteorder='big')
        return package