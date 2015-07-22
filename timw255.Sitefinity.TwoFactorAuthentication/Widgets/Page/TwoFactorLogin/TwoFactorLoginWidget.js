Type.registerNamespace("timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin");

timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget = function (element) {
    timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.initializeBase(this, [element]);

    this._submitButton = null;
    this._userNameTextField = null;
    this._passwordTextField = null;
    this._serviceUrl = null;
    this._returnUrlParam = null;
    this._handleRejectedUserParam = null;
    this._errorMessageLabel = null;
    this._icorrectLoginMessage = null;
    this._membershipProvider = null;
    this._rememberMeCheckbox = null;
    this._destinationPageUrl = null;

    this._submitButtonClickDelegate = null;
    this._textFieldKeyPressDelegate = null;
}

timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.prototype = {
    initialize: function () {
        timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.callBaseMethod(this, 'initialize');

        jQuery.support.cors = true;

        if (this.get_submitButton()) {
            this._submitButtonClickDelegate = Function.createDelegate(this, this._submitButtonClick);
            $addHandler(this.get_submitButton(), "click", this._submitButtonClickDelegate);
        }

        if (this.get_userNameTextField() && this.get_passwordTextField()) {
            this._textFieldKeyPressDelegate = Function.createDelegate(this, this._textFieldKeyPress);
            $addHandler(this.get_userNameTextField().get_element(), "keypress", this._textFieldKeyPressDelegate);
            $addHandler(this.get_passwordTextField().get_element(), "keypress", this._textFieldKeyPressDelegate);
        }
    },

    dispose: function () {
        if (this._submitButtonClickDelegate) {
            if (this.get_submitButton())
                $removeHandler(this.get_submitButton(), "click", this._submitButtonClickDelegate);

            delete this._submitButtonClickDelegate;
        }

        if (this._textFieldKeyPressDelegate) {
            if (this.get_userNameTextField() && this.get_userNameTextField().get_element())
                $removeHandler(this.get_userNameTextField().get_element(), "keypress", this._textFieldKeyPressDelegate);

            if (this.get_passwordTextField() && this.get_passwordTextField().get_element())
                $removeHandler(this.get_passwordTextField().get_element(), "keypress", this._textFieldKeyPressDelegate);

            delete this._textFieldKeyPressDelegate;
        }

        timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.callBaseMethod(this, 'dispose');
    },

    setEnabled: function (value) {
        this.get_submitButton().disabled = !value;
    },

    _textFieldKeyPress: function (event) {
        //if Enter is pressed
        if (13 == event.charCode) {
            this._submitButtonClick();
        }
    },

    _submitButtonClick: function () {
        if (this.get_submitButton().disabled) {
            return false;
        }

        this.get_errorMessageLabel().style.display = "none";

        if (!this.validate()) {
            return false;
        }

        this._sendAuthenticationRequest(this.get_userNameTextField().get_value(),
            this.get_passwordTextField().get_value(), this.get_serviceUrl());
    },

    _sendAuthenticationRequest: function (userName, password, serviceUrl) {
        //"that" is for use in the ajax request object
        //and it represents the LoginWidget
        var that = this;
        jQuery.ajax({
            type: "POST",
            url: serviceUrl,
            crossDomain: true,
            data: {
                realm: window.location.origin,
                redirect_uri: this._getReturnLocation(),
                wrap_name: userName,
                wrap_password: password,
                sf_domain: that._membershipProvider,
                sf_persistent: this.get_rememberMeCheckbox().checked,
                deflate: true
            },
            success: function (data, textStatus, jqXHR) {
                that._authenticationSuccess(data, textStatus, jqXHR);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //Unauthorized
                if (401 == jqXHR.status) {
                    that._showErrorMessage(that.get_incorrectLoginMessage());
                }
                else {
                    that._showErrorMessage(textStatus + ": " + jqXHR.responseText);
                }
            },
            complete: function (jqXHR, textStatus) {
                that.setEnabled(true);
            }
        });
    },

    _authenticationSuccess: function (data, textStatus, jqXHR) {
        var url = data.url;
        window.location = url;
    },

    _showErrorMessage: function (message) {
        this.get_errorMessageLabel().innerHTML = message;
        this.get_errorMessageLabel().style.display = "";
    },

    _getReturnLocation: function () {
        var returnLocation = "";
        var returnUrlParamIdx = window.location.search.indexOf(this.get_returnUrlParam());
        if (returnUrlParamIdx > -1) {
            var qsParams = this._getQueryStringParams();

            if (qsParams[this.get_returnUrlParam()] && qsParams[this.get_returnUrlParam()].length > 0) {
                var retUrl = qsParams[this.get_returnUrlParam()];

                if (retUrl.startsWith(window.location.protocol))
                    return retUrl;

                if (!retUrl.startsWith("/"))
                    retUrl = "/" + retUrl;

                returnLocation = window.location.protocol + "//" + window.location.host + retUrl;
            }
        }
        else {
            if (this.get_destinationPageUrl()) {
                returnLocation = this.get_destinationPageUrl();
            }
            else {
                returnLocation = document.URL;
            }
        }

        return returnLocation;
    },

    _getQueryStringParams: function () {
        var oGetVars = {};

        if (window.location.search.length > 1) {
            for (var aItKey, nKeyId = 0, aCouples = window.location.search.substr(1).split("&") ; nKeyId < aCouples.length; nKeyId++) {
                aItKey = aCouples[nKeyId].split("=");
                oGetVars[unescape(aItKey[0])] = aItKey.length > 1 ? unescape(aItKey[1]) : "";
            }
        }

        return oGetVars;
    },

    validate: function () {
        var validationResult = this.get_userNameTextField().validate();
        validationResult = this.get_passwordTextField().validate() && validationResult;
        return validationResult;
    },

    get_submitButton: function () {
        return this._submitButton;
    },
    set_submitButton: function (value) {
        this._submitButton = value;
    },

    get_userNameTextField: function () {
        return this._userNameTextField;
    },
    set_userNameTextField: function (value) {
        this._userNameTextField = value;
    },

    get_passwordTextField: function () {
        return this._passwordTextField;
    },
    set_passwordTextField: function (value) {
        this._passwordTextField = value;
    },

    get_serviceUrl: function () {
        return this._serviceUrl;
    },
    set_serviceUrl: function (value) {
        this._serviceUrl = value;
    },

    get_returnUrlParam: function () {
        return this._returnUrlParam;
    },
    set_returnUrlParam: function (value) {
        this._returnUrlParam = value;
    },

    get_handleRejectedUserParam: function () {
        return this._handleRejectedUserParam;
    },
    set_handleRejectedUserParam: function (value) {
        this._handleRejectedUserParam = value;
    },

    get_errorMessageLabel: function () {
        return this._errorMessageLabel;
    },
    set_errorMessageLabel: function (value) {
        this._errorMessageLabel = value;
    },

    get_incorrectLoginMessage: function () {
        return this._incorrectLoginMessage;
    },
    set_incorrectLoginMessage: function (value) {
        this._incorrectLoginMessage = value;
    },

    get_membershipProvider: function () {
        return this._membershipProvider;
    },
    set_membershipProvider: function (value) {
        this._membershipProvider = value;
    },

    get_rememberMeCheckbox: function () {
        return this._rememberMeCheckbox;
    },
    set_rememberMeCheckbox: function (value) {
        this._rememberMeCheckbox = value;
    },

    get_destinationPageUrl: function () {
        return this._destinationPageUrl;
    },
    set_destinationPageUrl: function (value) {
        this._destinationPageUrl = value;
    }
}

timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget.registerClass('timw255.Sitefinity.TwoFactorAuthentication.Widgets.Page.TwoFactorLogin.TwoFactorLoginWidget', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();