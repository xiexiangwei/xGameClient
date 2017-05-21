
public class PublicData: Singleton<PublicData>
{   
    //玩家账号ID
    public uint accountID;
    //玩家登陆token
    public string userToken;
    //玩家ID
    public uint userID;
    //玩家昵称
    public string userName;
    //玩家金币
    public uint userMoney;

    //游戏中心IP
    public string gcIP;
    //游戏中心端口
    public uint gcPort;
    
}
