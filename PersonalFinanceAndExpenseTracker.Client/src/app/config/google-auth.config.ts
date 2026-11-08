import { AuthConfig } from 'angular-oauth2-oidc';

export const googleAuthConfig: AuthConfig = {
  issuer: 'https://accounts.google.com',
  strictDiscoveryDocumentValidation: false,

  redirectUri: window.location.origin + '/signin-google',
  clientId: '',
  scope: 'openid profile email',
  
  customQueryParams: {
    prompt: 'select_account' 
  },
  showDebugInformation: true,
  requireHttps: false    
};