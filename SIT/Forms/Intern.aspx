<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Intern.aspx.cs" Inherits="SIT.Forms.Intern" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="MainFrame" runat="server" Height="906px" Visible="False">
        <asp:Button ID="logout" runat="server" onclick="logout_Click" Text="Logout" />
        <br />
        <br />
        <br />
        Datei auswählen:<br />
        <asp:FileUpload ID="UploadFile" runat="server" />
        <br />
        <br />
        <asp:Button ID="uploadButton" runat="server" onclick="uploadButton_Click" 
            Text="Verschlüsseln" />
        &nbsp;<asp:Button ID="decryptButton" runat="server" 
            onclick="decryptButton_Click" Text="Entschlüsseln" />
        <asp:SqlDataSource ID="GetMasterKey" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT [OwnerID], [MasterKey] FROM [MasterKeys] WHERE (([UserID] = @UserID) AND ([OwnerID] = @OwnerID))">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="ID" Type="Int32" />
                <asp:Parameter Name="OwnerID" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SelectPublicKey" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT Users.PublicKey, Users.PrivateKey, MasterKeys.MasterKey, MasterKeys.ID FROM Users INNER JOIN MasterKeys ON Users.ID = MasterKeys.UserID WHERE (Users.ID = @ID)">
            <SelectParameters>
                <asp:SessionParameter Name="ID" SessionField="ID" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
        <asp:Label ID="Error" runat="server" Text="Fehler" Visible="False"></asp:Label>
        <br />
        <br />
        Private Key Secret:<br /> &nbsp;<asp:TextBox ID="privateSecret" runat="server" 
            TextMode="Password"></asp:TextBox>
        <asp:SqlDataSource ID="checkPrivatePasswort" runat="server" 
            ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
            SelectCommand="SELECT [ID], [PrivateKeyPassword] FROM [Users] WHERE (([ID] = @ID) AND ([PrivateKeyPassword] = @PrivateKeyPassword))">
            <SelectParameters>
                <asp:SessionParameter Name="ID" SessionField="ID" Type="Int32" />
                <asp:Parameter Name="PrivateKeyPassword" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
        <br />
        <br />
        <asp:Button ID="KeyExchangeButton" runat="server" onclick="change_Click" 
            Text="Schlüsselverwaltung" />
        <br />
        <asp:Panel ID="KeyExchangePanel" runat="server" Visible="False">
            <br />
            Sie besitzen die Schlüssel von folgenden Benutzern:<br />
            <asp:GridView ID="AllKeysView" runat="server" AutoGenerateColumns="False" 
                DataSourceID="SelectAllKeys">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SelectAllKeys" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
                SelectCommand="SELECT Users.name AS Name FROM MasterKeys INNER JOIN Users ON MasterKeys.OwnerID = Users.ID WHERE (MasterKeys.UserID = @user )">
                <SelectParameters>
                    <asp:SessionParameter Name="user" SessionField="ID" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            Schlüssel weitergeben:<br />
            <br />
            <asp:DropDownList ID="UserSelect" runat="server" DataSourceID="SelectAllUsers" 
                DataTextField="Name" DataValueField="ID">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SelectAllUsers" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
                SelectCommand="SELECT [name] as Name, [ID] FROM [Users]">
            </asp:SqlDataSource>
            <br />
            <br />
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                Text="Weitergeben" />
            <br />
            <asp:SqlDataSource ID="KeyExchange" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
                InsertCommand="INSERT INTO MasterKeys(UserID, MasterKey, IsCopy, OwnerID) VALUES (@UserID, @MasterKey, 1, @OwnerID)" 
                
                SelectCommand="SELECT ID, MasterKey, IsCopy, OwnerID, UserID FROM MasterKeys WHERE (UserID = @UserID) AND (OwnerID = @OwnerID)">
                <InsertParameters>
                    <asp:Parameter Name="UserID" />
                    <asp:Parameter Name="MasterKey" />
                    <asp:SessionParameter Name="OwnerID" SessionField="ID" />
                </InsertParameters>
                <SelectParameters>
                    <asp:Parameter Name="UserID" />
                    <asp:SessionParameter Name="OwnerID" SessionField="ID" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SelectPublicKeyOfUser" runat="server" 
                ConnectionString="<%$ ConnectionStrings:SIT_Database %>" 
                SelectCommand="SELECT [ID], [PublicKey] FROM [Users] WHERE ([ID] = @ID)">
                <SelectParameters>
                    <asp:Parameter Name="ID" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <br />
        </asp:Panel>
    </asp:Panel>
    <br />
    </form>
</body>
</html>
