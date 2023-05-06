using Surreal.Net.Test.Entities;

namespace Surreal.Net.Test;

[TestClass]
public class SurrealTest
{
    const string SURREAL_URL = "ws://127.0.0.1:8000/rpc/";
    [TestMethod]
    public void CreateTest()
    {

        var db = new Surreal(SURREAL_URL);

        

        db.SignIn("root", "root");

        db.Use("test", "test");

        db.Ping();

        var cust = new Customer{
            Balance = 10,
            Name = "Jonny",
            BirthDate = new DateTime(1999, 10, 10)
        };

        var result = db.Create<Customer>(cust.GetType().Name, cust);
        Assert.IsNull(result.Error);
        Assert.IsNotNull(result.Result[0].Id);

        Assert.AreEqual(cust.Balance, result.Result[0].Balance);
        Assert.AreEqual(cust.Name, result.Result[0].Name);
        Assert.AreEqual(cust.BirthDate, result.Result[0].BirthDate);


        var created = db.Select<Customer>(cust.GetType().Name);
        Assert.AreEqual(cust.Balance, created.Result[0].Balance);
        Assert.AreEqual(cust.Name, created.Result[0].Name);
        Assert.AreEqual(cust.BirthDate, created.Result[0].BirthDate);

        

    }
}