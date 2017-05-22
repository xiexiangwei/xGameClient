using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSocket.ClientEngine;
using System.Net;
using System;
using ProtoBuf;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginInit : MonoBehaviour
{

	//管理服务器客户端
	public static SMServer smClient;//管理服务器客户端
	public static LGServer lgClient;//登录网关客户端
	public static bool canLogin = false;//是否可以请求登陆
	public static bool loadGCScene = false;//是否加载游戏中心场景

	public static void Send2LoginGate(Const.CMD cmd, byte[] data)
	{
		lgClient.SendCmd(cmd,data);
	}

	void Start()
	{
		Debug.Log("LoginInit Start()");
		smClient = new SMServer();
		smClient.Start("127.0.0.1", 1111);
	}

	private void Update()
	{
		if(loadGCScene)
		{
			loadGCScene = false;
			//加载游戏中心场景
			SceneManager.LoadSceneAsync("GameCenterScene");
		}
	}

	private void OnDestroy()
	{
		Debug.Log("OnDestroy");

		//断开管理服务器连接
		if (smClient != null)
		{
			smClient.Close();
		}
		//断开登录网关连接
		if (lgClient != null)
		{
			lgClient.Close ();
		}

	}
}
