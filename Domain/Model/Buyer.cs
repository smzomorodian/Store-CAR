using Domain.Model;
using FirebaseAdmin.Auth;
public class Buyer : User
{
    public Buyer() { }

    public Buyer(string name, string age, string nationalcode, string password, string phonenumber, string role)
        : base(name, age, nationalcode, password, phonenumber, role)
    {
    }

}
