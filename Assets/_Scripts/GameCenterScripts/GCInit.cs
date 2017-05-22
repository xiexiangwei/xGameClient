using SuperSocket.ClientEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System;
using ProtoBuf;
using System.IO;

public class GCInit : MonoBehaviour {

	public static GCServer gcClient;//游戏中心客户端
    public Text userIDText;//玩家ID标签
    public Text userNameText;//玩家昵称标签
    public Text moneyText;//玩家金币标签
	//委托
	public delegate void ShowUserInfoDelegate(CmdMessage.RePly_Enter_GameCenter data);
	public ShowUserInfoDelegate ShowUserInfo;

    void Start ()
    {   
		ShowUserInfo = ShowUserInfoFunc;
        //连接游戏中心
		gcClient = new GCServer (PublicData.Instance.gcIP, (int)PublicData.Instance.gcPort);
		gcClient.Start ();
    }
  
	private void  ShowUserInfoFunc(CmdMessage.RePly_Enter_GameCenter data)
	{
		userIDText.text = string.Format("ID:{0}", data.user_info.user_id);
		userNameText.text = data.user_info.user_name;
		moneyText.text = string.Format("金币:{0}", data.user_info.money);
	}


    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        //断开游戏中心连接
        if (gcClient != null)
        {
			gcClient.Close ();
        }
    }

}
