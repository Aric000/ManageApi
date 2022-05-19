namespace Domain.Interface;

public interface IUserInfo
{
    /// <summary>
    /// 是否登录成功
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    bool IsLoginSuccess(string userName, string password);
}