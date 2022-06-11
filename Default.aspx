<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebRole1._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title runat="server" ><%= Page.Title %></title>
        <style type="text/css">
            .auto-style1 {
                width: 164px;
            }
        </style>
    </head>

<body>
    <h1>Guestbook</h1>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        Name
                    </td>
                    <td class="auto-style1">
                    <asp:TextBox ID="Name" runat="server"></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td>
                        Message
                    </td>
                     <td class="auto-style1">
                    <asp:TextBox ID="Message" TextMode="MultiLine" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Upload
                    </td>
                    <td class="auto-style1">
                        <asp:FileUpload ID="PictureUpload" runat="server"  />
                </td>
                </tr>
                <tr>
                     <td class="auto-style1">
                    <asp:Button ID="SubmitButton" Text="Submit" runat="server" OnClick="Button1_Click"></asp:Button>
                         <asp:Timer ID="Timer1" runat="server" Enabled="False" Interval="15000" OnTick="Timer1_Tick1">
                         </asp:Timer>
                    </td>
                </tr>

            </table>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
         </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:DataList ID="ImageList" runat="server"  DataSourceID="ObjectDataSource1">
                    <ItemTemplate>
                        <div class="DataItem">
                            <div class="Image">
                                <a href="<%# Eval("PhotoUrl") %>" target="_blank">
                                    <img src="<%# Eval("ThumbnailUrl") %>" alt="GuestName" />
                                </a>
                            </div>
                            <div class="Description">
                                <div class="Name">
                                    <%# Eval("GuestName") %>
                                </div>
                                <div class="Message">
                                    <%# Eval("Message") %>
                                </div>
                            </div>
                    </ItemTemplate>
            </asp:DataList>
            <asp:ObjectDataSource ID="ObjectDataSource1" DataObjectTypeName="Data.DataEntry" runat="server" SelectMethod="GetEntries" TypeName="Data.DataSource">
            </asp:ObjectDataSource>
            </ContentTemplate>

        </asp:UpdatePanel>
        
             </div>
    </form>
</body>
</html>
