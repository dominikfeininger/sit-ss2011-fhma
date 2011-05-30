<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="SIT.Forms.Register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

        .style1
        {
            width: 50%;
            text-align: right;
        }
    </style>
</head>
<body bgcolor="#00496c" style="height: 614px">
    <form id="registerForm" runat="server">
        <div style="height: 40px; margin:50px; text-align: center;">
        <div style="height: 40px; margin:20px 0px 0px 0px; text-align: center;">
        <div style="padding: 10px; border: thin solid #2380B1; background-color: #005B88; margin-top: 10px; z-index: auto; font-family: Arial, Helvetica, Sans-Serif; color: #FFFFFF;">
            <strong style="font-family: Arial, Helvetica, Sans-Serif">Registrieren<br />
            </strong><br />
            <table style="width:100%;">
                <tr>
                    <td class="style1">
                        Benutzername:&nbsp;&nbsp;&nbsp; 
                    </td>
                    <td style="text-align: left">
            <asp:TextBox ID="UserTextbox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        Passwort:&nbsp;&nbsp;&nbsp;
                    </td>
                    <td style="text-align: left">
            <asp:TextBox ID="PasswordTextbox1" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        Passwort bestätigen:&nbsp;&nbsp;&nbsp;&nbsp; </td>
                    <td style="text-align: left">
            <asp:TextBox ID="PasswordTextbox2" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        Private Key Passwort:&nbsp;&nbsp;&nbsp;&nbsp; </td>
                    <td style="text-align: left">
            <asp:TextBox ID="PrivateKeyPassword1" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        Private Key Passwort bestätigen:&nbsp;&nbsp;&nbsp;&nbsp; </td>
                    <td style="text-align: left">
            <asp:TextBox ID="PrivateKeyPassword2" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        &nbsp;</td>
                    <td style="text-align: left">
                        <br />
            <asp:Button ID="registerButton" runat="server" Text="Registrieren" 
                onclick="registerButton_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Label ID="ErrorLabel" runat="server" 
                style="color: #FFFFFF; background-color: #005B88" Text="Error" 
                Visible="False" BackColor="#005B88"></asp:Label>
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
        </div>
        </div>
     </div>
    </form>
</body>
</html>
