import { AuthConfig } from 'angular-oauth2-oidc';

export const googleAuthConfig: AuthConfig = {
  issuer: 'https://accounts.google.com',
  strictDiscoveryDocumentValidation: false,

  redirectUri: window.location.origin + '/signin-google',
  clientId: '893618120730-0bhgi3euf0vs2hul151erhtjpopgner4.apps.googleusercontent.com',
  scope: 'openid profile email',
  
  customQueryParams: {
    prompt: 'select_account' 
  },
  showDebugInformation: true,
  requireHttps: false    
};