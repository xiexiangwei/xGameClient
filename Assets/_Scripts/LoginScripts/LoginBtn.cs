using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ProtoBuf;

public class LoginBtn : MonoBehaviour 
{
    public InputField accountNameInput;
    public InputField accountPwdInput;
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
				if( LoginInit.canLogin)
				{
					//请求登录
					CmdMessage.Request_Login req_login = new CmdMessage.Request_Login();
					req_login.account_name = accountNameInput.text;
					req_login.account_pwd = accountPwdInput.text;
					Debug.Log(string.Format("请求登陆,account_name:{0} pwd:{1}", req_login.account_name, req_login.account_pwd));
					//序列化操作
					MemoryStream ms = new MemoryStream();
					Serializer.Serialize<CmdMessage.Request_Login>(ms, req_login);
					LoginInit.Send2LoginGate(Const.CMD.C2LG_Login, ms.ToArray());
				}
				else
				{
					Debug.Log("不能请求登陆!");
				}
               
            }
        };
    }
}
