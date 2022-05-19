using Domain.Interface;

namespace Infrastructure;

public class UserInfo : IUserInfo
{
    /// <summary>
    /// <see cref="IUserInfo.IsLoginSuccess"/>
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool IsLoginSuccess(string userName, string password)
    {
        var sql = $"select count(1) from base_user where user_name ='{userName}' and password ='{password}'";
        return MySqlHelper.ExecuteNonQuery(sql) > 0;
    }
}