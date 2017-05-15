public static class Const
{
    //=======ErrorCode
    public enum ERROR_CODE
    {
       ERROR_OK=0,
       ERROR_SERVER=1, 
       ERROR_MAX_LOGINGATE=2,
       ERROR_NO_LOGINGATE=4,
       ERROR_NOT_READY_LOGIN=5,
       ERROR_ACCOUNT_NOT_EXISTS=6,
       ERROR_ACCOUNT_PWD_ERROR=7
    }

    public enum CMD
    {
        //#=======心跳包
        KEEPLIVE = 1,

        //#客戶端--->管理服务器消息ID:[501-600]
        C2SM_GET_LOGINGATE = 501,

        //#管理服务器--->客戶端消息ID:[601-700]
        SM2C_GET_LOGINGATE_REPLY = 601,

        //#客户端--->登录网关消息ID:[701-800]
        C2LG_Login = 701,

        //#登录网关--->客户端消息ID:[801-900]
        LG2C_READY_TO_LOGIN=801,
        LG2C_LOGIN_RESULT = 802,

    }

}
