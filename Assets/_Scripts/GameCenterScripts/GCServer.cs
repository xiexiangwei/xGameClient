using UnityEngine;
using System.IO;
using ProtoBuf;

public class GCServer:SocketClientBase
{
    protected override void OnConnected()
    {
        Debug.Log("游戏中心连接成功");
        //连接成功,请求进入游戏中心
        CmdMessage.Request_Enter_GameCenter req = new CmdMessage.Request_Enter_GameCenter();
        req.account_id = PublicData.Instance.accountID;
        req.token = PublicData.Instance.userToken;
        Debug.Log(string.Format("请求进入游戏中心,account_id:{0} token:{1}", req.account_id, req.token));
        //序列化操作
        MemoryStream ms = new MemoryStream();
        Serializer.Serialize(ms, req);
        SendCmd(Const.CMD.C2GC_REQUEST_ENTER_GC, ms.ToArray());
    }

    protected override void CmdParse(uint cmdid, byte[] body)
    {
        switch ((Const.CMD)cmdid)
        {
            case Const.CMD.GC2C_REPLY_ENTER_GC:
                {
                    var data = Serializer.Deserialize<CmdMessage.RePly_Enter_GameCenter>(new MemoryStream(body));
                    if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
                    {
                        Debug.Log(string.Format("进入游戏中心成功! user_id:{0} user_name:{1} money:{2} game_list:{1}",
                            data.user_info.user_id, data.user_info.user_name, data.user_info.money));

                        PublicData.Instance.userID = data.user_info.user_id;
                        PublicData.Instance.userName = data.user_info.user_name;
                        PublicData.Instance.userMoney = data.user_info.money;
                        for (int i = 0; i < data.game_list.Count; i++)
                        {
                            Debug.Log(string.Format("游戏{0} 类型:{1} 服务器IP:{2} 端口:{3}", i, data.game_list[i].game_type, data.game_list[i].game_ip, data.game_list[i].game_port));
                        }
                        //显示玩家信息
                        GCInit.showUserInfo = true;
                    }
                    else
                    {
                        Debug.Log("进入游戏中心失败!");
                    }
                }
                break;
        }
    }
}
