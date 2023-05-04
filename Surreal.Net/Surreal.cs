namespace Surreal.Net;
public class Surreal
{
    private readonly string _url = "";

    public Surreal(string url)
    {
        _url = url;
    }

    public void SignIn(string user, string pass)
    {

    }

    public void Use(string ns, string db)
    {

    }

    public T Create<T>(T obj) where T: class, new()
    {
        return obj;
    }

    public T Change<T>(object obj) where T: class, new()
    {
        return new T();
    }
}
