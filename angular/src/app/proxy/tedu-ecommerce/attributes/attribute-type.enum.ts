import { mapEnumToOptions } from '@abp/ng.core';

export enum AttributeType {
  Date = 1,
  Varchar = 2,
  Text = 3,
  Int = 4,
}

export const attributeTypeOptions = mapEnumToOptions(AttributeType);
