<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SIT.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 419px; text-align: center">
    <form id="loginForm" runat="server">
    <div style="margin:0px 0px 10px 0px;">
        <div style="height: 40px; margin:100px 0px 0px 0px;">
        <asp:Panel ID="Panel1" runat="server">
            Benutzer:
            <asp:TextBox ID="UserTextbox" runat="server"></asp:TextBox>
        </asp:Panel>
        </div>
        <div style="height: 40px;">
        <asp:Panel ID="Panel2" runat="server">
            Passwort:
            <asp:TextBox ID="PasswordTextbox" runat="server" style="text-align: left" 
                TextMode="Password"></asp:TextBox>
        </asp:Panel>
         </div>
            <asp:Panel ID="Panel3" runat="server">
                <asp:Button ID="Anmelden" runat="server" Text="Anmelden" 
                    onclick="Anmelden_Click" />
            </asp:Panel>
        <asp:SqlDataSource ID="DataSourceCheckPW" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT * FROM [Users] WHERE (([Name] = @Name) AND ([Password] = @Password))">
            <SelectParameters>
                <asp:ControlParameter ControlID="UserTextbox" Name="Name" PropertyName="Text" 
                    Type="String" />
                <asp:Parameter Name="Password" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Label ID="ErrorLabel" runat="server" style="color: #FF0000" Text="Error" 
            Visible="False"></asp:Label>
    </div>
    </form>
</body>
</html>
