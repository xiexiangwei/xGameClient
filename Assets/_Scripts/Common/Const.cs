public static class Const
{
    //=======ErrorCode
    public enum ERROR_CODE
    {
        ERROR_OK = 0,
        ERROR_SERVER = 1,
        ERROR_MAX_LOGINGATE = 2,
        ERROR_NO_LOGINGATE = 4,
        ERROR_NOT_READY_LOGIN = 5,
        ERROR_ACCOUNT_NOT_EXISTS = 6,
        ERROR_ACCOUNT_PWD_ERROR = 7
    }

    public enum CMD
    {
        //#=======心跳包
        KEEPLIVE = 1,

        //#客戶端--->管理服务器消息ID:[501-600]
        C2SM_GET_LOGINGATE = 501,
        C2SM_GET_GAMECENTER = 502,  //# 请求获得游戏中心服务器信息

        //#管理服务器--->客戶端消息ID:[601-700]
        SM2C_GET_LOGINGATE_REPLY = 601,
        SM2C_GET_GAMECENTER_REPLY = 602, //# 请求获得游戏中心服务器信息返回

        //#客户端--->登录网关消息ID:[701-800]
        C2LG_Login = 701,

        //#登录网关--->客户端消息ID:[801-900]
        LG2C_READY_TO_LOGIN = 801,
        LG2C_LOGIN_RESULT = 802,


        //# 客户端--->游戏中心消息ID:[3001-4000]
        C2GC_REQUEST_ENTER_GC = 3001, //# 玩家进入游戏中心

        //# 游戏中心--->客户端消息ID:[4001-5000]
        GC2C_REPLY_ENTER_GC = 4001, //# 玩家进入游戏中心返回

        //# 客户端--->炸金花服务器消息:[5001-6000]
        C2TCS_REQUEST_ENTERGAME = 5001,  //# 客户端请求进入炸金花服务器
        C2TCS_REQUEST_ROOM_TABLES_INFO = 5002, // # 请求房间内桌子信息
        C2TCS_REQUEST_SIT = 5003, // # 请求坐下

        //# 炸金花服务器--->客户端消息:[6001-7000]
        TCS2C_REPLY_ENTERGAME = 6001, // # 进入炸金花游戏返回
        TCS2C_REPLY_ROOM_TABLES_INFO = 6002, // # 请求房间内桌子信息返回
        TCS2C_REPLY_SIT = 6003,  //# 请求坐下返回
        TCS2C_BROADCAST_USER_SIT = 6004,  //# 广播玩家加入桌子


    }

}
