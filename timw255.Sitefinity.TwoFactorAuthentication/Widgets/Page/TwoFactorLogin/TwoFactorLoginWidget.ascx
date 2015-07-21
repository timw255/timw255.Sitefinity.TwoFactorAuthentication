<%@ Control Language="C#" %>

<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI" TagPrefix="sitefinity" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.Fields" TagPrefix="sfFields" %>

<sfFields:FormManager id="formManager" runat="server" />

<fieldset id="sfLoginWidgetWrp">
    <asp:Panel ID="loginWidgetPanel" runat="server">
        <ol class="sfLoginFieldsWrp">
			<sfFields:TextField ID="UserName" AccessKey="u" runat="server" DisplayMode="Write" WrapperTag="li">
                <ValidatorDefinition Required="true" 
                            MessageCssClass="sfError" 
                            RequiredViolationMessage="<%$ Resources:Labels, UsernameCannotBeEmpty %>"/>  
            </sfFields:TextField>
            <sfFields:TextField ID="Password" IsPasswordMode="true" AccessKey="p" runat="server" DisplayMode="Write" WrapperTag="li">
                <ValidatorDefinition Required="true" 
                            MessageCssClass="sfError" 
                            RequiredViolationMessage="<%$ Resources:Labels, PasswordCannotBeEmpty %>"/>  
            </sfFields:TextField>
            <li class="sfCheckBoxWrapper">
                <asp:CheckBox runat="server" ID="rememberMeCheckbox" Checked="true" />
                <asp:Label runat="server" AssociatedControlID="rememberMeCheckbox" Text="<%$ Resources:Labels, RememberMe %>" />
            </li>
	    </ol>
	    <div class="sfSubmitBtnWrp">
		    <asp:LinkButton ID="LoginButton" CssClass="sfSubmitBtn" ValidationGroup="LoginBox" OnClientClick="return false;" runat="server">
			    <asp:Literal ID="LoginButtonLiteral" runat="server"></asp:Literal>
		    </asp:LinkButton>
            <asp:LinkButton ID="lostPasswordBtn" runat="server" Text="<%$ Resources:Labels, ForgotYourPassword %>" CssClass="sfLostPassword" />
            <asp:LinkButton ID="RegisterUserBtn" runat="server" CssClass="sfLostPassword">
                <asp:Literal ID="RegisterUserText" runat="server" />
            </asp:LinkButton>
	    </div>
        <sitefinity:SitefinityLabel ID="ErrorMessageLabel" WrapperTagName="div" runat="server" CssClass="sfError" />
    </asp:Panel>
    <asp:Panel ID="lostPasswordPanel" runat="server" Visible="false" CssClass="sfLoginFieldsWrp sfLostPasswordWrp">
        <h2 class="sfLoginFieldsTitle"><asp:Literal ID="literal1" runat="server" Text="<%$ Resources:Labels, ForgotYourPassword %>" /></h2>
        <p class="sfLoginFieldsNote"><asp:Literal ID="literal2" runat="server" Text="<%$ Resources:Labels, ToResetYourPasswordEnterYourEmail %>" /></p>
        <div>
            <asp:Label ID="literal3" runat="server" Text="<%$ Resources:Labels, EmailAddress %>" AssociatedControlID="mailTextBox" CssClass="sfTxtLbl" />
            <asp:TextBox ID="mailTextBox" runat="server" ValidationGroup="loginWidget" CssClass="sfTxt" />
            <asp:RequiredFieldValidator 
                id="mailRequiredFieldValidator" runat="server" Display="Dynamic"
                ValidationGroup="loginWidget"
                ControlToValidate="mailTextBox">
                <div class="sfError"><asp:Literal runat="server" ID="lTheMailFieldIsRequired" Text="<%$Resources:Labels, Required%>" /></div>                
            </asp:RequiredFieldValidator>
            <sitefinity:SitefinityLabel ID="lostPasswordError" WrapperTagName="div" runat="server" CssClass="sfError" />
        </div>
        <div class="sfSubmitBtnWrp">
            <asp:Button ID="sendRecoveryMailBtn" runat="server" Text="<%$ Resources:Labels, Submit %>" ValidationGroup="loginWidget" CssClass="sfSubmitBtn" />
        </div>
    </asp:Panel>
    <asp:Panel ID="passwordResetSentPanel" runat="server" Visible="false">
        <asp:Literal ID="literal4" runat="server" Text="<%$ Resources:Labels, PasswordResetInstructionsHaveBeenSentToYourEmail %>" />
    </asp:Panel>
</fieldset>