<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lti1113.aspx.cs" Inherits="OAuth2POC.Lti1113" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>

    <table>
        <tr>
            <td><strong>Course name: </strong></td>
            <td><asp:Label ID="lblCourseName" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>API Domain: </strong></td>
            <td><asp:Label ID="lblApiDomain" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Course Id: </strong></td>
            <td><asp:Label ID="lblCourseId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Course SIS Id: </strong></td>
            <td><asp:Label ID="lblCourseSisId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>User Id: </strong></td>
            <td><asp:Label ID="lblUserId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>User SIS Id: </strong></td>
            <td><asp:Label ID="lblUserSisId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>User Login Id: </strong></td>
            <td><asp:Label ID="lblLoginId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>External Tool Id: </strong></td>
            <td><asp:Label ID="lblExtToolId" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Launch Presentation URL: </strong></td>
            <td><asp:Label ID="lblLaunchPresentationUrl" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Common CSS URL: </strong></td>
            <td><asp:Label ID="lblCommonCssUrl" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Is Course Instructor: </strong></td>
            <td><asp:Label ID="lblIsCourseInstructor" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Is Admin: </strong></td>
            <td><asp:Label ID="lblIsAdmin" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Is Course Admin: </strong></td>
            <td><asp:Label ID="lblIsCourseAdmin" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td><strong>Is Group Admin: </strong></td>
            <td><asp:Label ID="lblIsGroupAdmin" runat="server"></asp:Label></td>
        </tr>
        
    </table>

    
</asp:Content>
