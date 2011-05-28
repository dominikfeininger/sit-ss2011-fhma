<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="SIT.Forms.Register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="registerForm" runat="server">
        <div style="height: 40px; margin:100px 0px 0px 0px; text-align: center;">
        <asp:Panel ID="Panel1" runat="server">
            <asp:SqlDataSource ID="SqlAddNewUser" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" InsertCommand="INSERT INTO Users(name, password, PublicKey, PrivateKey, PrivateKeyPassword) 

VALUES (@name,@password,@PublicKey,@PrivateKey,@PrivateKeyPassword)" 
                ProviderName="<%$ ConnectionStrings:SIT_Database.ProviderName %>" 
                SelectCommand="SELECT ID, name FROM Users WHERE (name = @name)">
                <InsertParameters>
                    <asp:Parameter Name="name" />
                    <asp:Parameter Name="password" />
                    <asp:Parameter Name="PublicKey" />
                    <asp:Parameter Name="PrivateKey" />
                    <asp:Parameter Name="PrivateKeyPassword" />
                </InsertParameters>
                <SelectParameters>
                    <asp:Parameter Name="name" />
                </SelectParameters>
            </asp:SqlDataSource>
            Benutzername:
            <asp:TextBox ID="UserTextbox" runat="server"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server">
            Passwort:
            <asp:TextBox ID="PasswordTextbox1" runat="server" TextMode="Password"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel3" runat="server">
            Passwort bestätigen:
            <asp:TextBox ID="PasswordTextbox2" runat="server" TextMode="Password"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel5" runat="server">
            Private-Key Passwort:
            <asp:TextBox ID="PrivateKeyPassword1" runat="server" TextMode="Password"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel6" runat="server">
            Private-Key Passwort bestätigen:
            <asp:TextBox ID="PrivateKeyPassword2" runat="server" TextMode="Password"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel4" runat="server">
            <asp:Button ID="registerButton" runat="server" Text="Registrieren" 
                onclick="registerButton_Click" />
        </asp:Panel>
        <div style="height: 40px; margin:20px 0px 0px 0px; text-align: center;">
            <asp:Label ID="ErrorLabel" runat="server" 
                style="color: #FF0000; background-color: #FFFFFF" Text="Error" Visible="False"></asp:Label>
        </div>
     </div>
        <asp:SqlDataSource ID="SqlAddNewMasterkey" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            InsertCommand="INSERT INTO MasterKeys(UserID, MasterKey, IsCopy, OwnerID) VALUES (@UserID, @Masterkey, @IsCopy, @OwnerID)" 
            
            SelectCommand="SELECT [ID], [MasterKey], [UserID], [IsCopy] FROM [MasterKeys]">
            <InsertParameters>
                <asp:Parameter Name="UserID" />
                <asp:Parameter DefaultValue="" Name="Masterkey" />
                <asp:Parameter DefaultValue="0" Name="IsCopy" />
                <asp:Parameter Name="OwnerID" />
            </InsertParameters>
        </asp:SqlDataSource>
    </form>
</body>
</html>
