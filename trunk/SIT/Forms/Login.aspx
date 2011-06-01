<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SIT.Login" %>

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
        .style2
        {
            width: 33%;
        }
        .style3
        {
            width: 33%;
        }
    </style>
</head>
<body style="height: 333px; text-align: center; font-family: Arial, Helvetica, Sans-Serif; color: #FFFFFF; margin:50px" 
    bgcolor="#00496c">
    <form id="loginForm" runat="server">
    <div style="margin:0px 0px 10px 0px;">
            <asp:Panel ID="Panel3" runat="server">
            </asp:Panel>
        <div style="padding: 10px; border: thin solid #2380B1; background-color: #005B88; margin-top: 10px; z-index: auto;">
            <strong>Login<br />
            </strong><br />
            <table style="width:100%;">
                <tr>
                    <td class="style1">
                        Benutzername:&nbsp;&nbsp;&nbsp; 
                    </td>
                    <td style="text-align: left">
            <asp:TextBox ID="UserTextbox" runat="server" style="text-align: left"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        Passwort:&nbsp;&nbsp;&nbsp;
                    </td>
                    <td style="text-align: left">
            <asp:TextBox ID="PasswordTextbox" runat="server" style="text-align: left" 
                TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        &nbsp;</td>
                    <td style="text-align: left">
                        <br />
                <asp:Button ID="Anmelden" runat="server" Text="Anmelden" 
                    onclick="Anmelden_Click" />
                    </td>
                </tr>
            </table>
            <br />
        <asp:Label ID="ErrorLabel" runat="server" style="color: #FFFFFF" Text="Error" 
            Visible="False"></asp:Label>
        </div>
    </div>
        <asp:SqlDataSource ID="DataSourceCheckPW" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT * FROM [Users] WHERE (([Name] = @Name) AND ([Password] = @Password))">
            <SelectParameters>
                <asp:ControlParameter ControlID="UserTextbox" Name="Name" PropertyName="Text" 
                    Type="String" />
                <asp:Parameter Name="Password" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    <div style="padding: 10px 0px 0px 0px; border: thin solid #2380B1; background-color: #005B88; margin-top: 20px; z-index: auto; text-align: left;">
            &nbsp;&nbsp; <strong>Datei über Header entschlüsseln</strong>
            <div style="border-color: #2380B1; margin-top: 10px; padding: 10px; background-color: #106898; border-top-style: solid;">
                <div style="border: thin dashed #5BB1DF; padding: 5px; background-color: #0077B0;">
                    Die gewählte Datei wird ohne Datenbankanbindung entschlüsselt
                    <div 
                        style="border: thin solid #0D557B; margin: 7px 0px 0px 0px; padding: 5px; background-color: #116C9D;">
                        <table style="width:100%;">
                            <tr>
                                <td class="style2">
                                    <asp:FileUpload ID="UploadFile" runat="server" />
                                </td>
                                <td style="text-align: center" class="style3">
                                    Super-Passwort:
                                    <asp:TextBox ID="SuperPW" runat="server" TextMode="Password"></asp:TextBox>
                                </td>
                                <td style="text-align: right">
                                    <asp:Button ID="DecryptButton" runat="server" 
                                        onclick="DecryptWithHeaderButton_Click" Text="Entschlüsseln" />
                                </td>
                            </tr>
                        </table>
                </div>
            </div>
        </div>
        </div>
    </form>
    </body>
</html>
