using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ProtoBuf;

public class LoginBtn : MonoBehaviour 
{

	// Use this for initialization
    void Start()
    {
        Debug.Log("LoginBtn Start()");
        Button btn = this.GetComponent<Button>();
        UIEventListener btnListener = btn.gameObject.AddComponent<UIEventListener>();

        btnListener.OnClick += delegate(GameObject gb)
        {
            Debug.Log(gb.name + " OnClick");
            if (gb.name=="LoginButton")
            {   
                //请求登录
                CmdMessage.Request_Login req_login = new CmdMessage.Request_Login();
                req_login.account_name = "test";
                req_login.account_pwd = "123456";
                //序列化操作
                MemoryStream ms = new MemoryStream();
                Serializer.Serialize<CmdMessage.Request_Login>(ms, req_login);
                LoginInit.Send2LoginGate(Const.CMD.C2LG_Login, ms.ToArray());
            }
        };
    }
}
