<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="AFDApp.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<div>
<asp:Label ID="lblProcessing" runat="server" Text="We are processing your request. Please don't close this browser." />
<br />
<asp:Label ID="lblResult" runat="server" Visible="false" Font-Bold="true" />
<br />
</div>
</asp:Content>
