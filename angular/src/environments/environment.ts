import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'TeduEcommerce Admin',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:5000/',
    redirectUri: baseUrl,
    clientId: 'TeduEcommerce_Admin',
    responseType: 'code',
    scope: 'offline_access TeduEcommerce.Admin',
    requireHttps: true,
    dummyClientSecret: '1q2w3e*',
  },
  apis: {
    default: {
      url: 'https://localhost:5001',
      rootNamespace: 'TeduEcommerce.Admin',
    },
  },
} as Environment;
