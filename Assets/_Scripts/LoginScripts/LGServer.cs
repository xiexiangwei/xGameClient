using UnityEngine;
using ProtoBuf;
using System.IO;
/// <summary>
/// LG server.登录网关类
/// </summary>
public class LGServer : SocketClientBase
{

    protected override void OnConnected()
    {
        Debug.Log("登录网关连接成功");
    }

    protected override void CmdParse(uint cmdid, byte[] body)
    {
        switch ((Const.CMD)cmdid)
        {
            case Const.CMD.LG2C_READY_TO_LOGIN:
                {
                    var data = Serializer.Deserialize<CmdMessage.Reply_Connect_Logingate>(new MemoryStream(body));
                    if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
                    {
                        Debug.Log("可以请求登录或者注册!");
                        LoginInit.canLogin = true;
                    }
                    else
                    {
                        Debug.Log("没有可用的登录服务器!");
                    }
                }
                break;
            case Const.CMD.LG2C_LOGIN_RESULT:
                {
                    var data = Serializer.Deserialize<CmdMessage.Reply_Login>(new MemoryStream(body));
                    if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
                    {
                        Debug.Log(string.Format("登录成功！ account_id:{0} token:{1}", data.account_id, data.token));
                        PublicData.Instance.accountID = data.account_id;
                        PublicData.Instance.userToken = data.token;
                        //请求管理服务器获取游戏中心地址
                        CmdMessage.Request_Get_GameCenter req = new CmdMessage.Request_Get_GameCenter();
                        req.account_id = data.account_id;
                        req.token = data.token;
                        Debug.Log(string.Format("请求获取游戏中心信息 account_id:{0} token:{1}", req.account_id, req.token));
                        MemoryStream ms = new MemoryStream();
                        Serializer.Serialize(ms, req);
                        LoginInit.smClient.SendCmd(Const.CMD.C2SM_GET_GAMECENTER, ms.ToArray());
                    }
                    else
                    {
                        if (data.error == (int)Const.ERROR_CODE.ERROR_SERVER)
                        {
                            Debug.Log("游戏服务器逻辑出错!");
                        }
                        else if (data.error == (int)Const.ERROR_CODE.ERROR_ACCOUNT_NOT_EXISTS)
                        {
                            Debug.Log("帐号不存在!");
                        }
                        else if (data.error == (int)Const.ERROR_CODE.ERROR_ACCOUNT_PWD_ERROR)
                        {
                            Debug.Log("密码错误!");
                        }
                    }
                }
                break;
        }
    }

}
