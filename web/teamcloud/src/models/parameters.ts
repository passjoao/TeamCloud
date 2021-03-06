/*
 * Copyright (c) Microsoft Corporation.
 * Licensed under the MIT License.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator.
 * Changes may cause incorrect behavior and will be lost if the code is regenerated.
 */

import {
  OperationParameter,
  OperationURLParameter,
  OperationQueryParameter
} from "@azure/core-http";
import {
  ProjectDefinition as ProjectDefinitionMapper,
  ComponentRequest as ComponentRequestMapper,
  ProjectLink as ProjectLinkMapper,
  ProviderData as ProviderDataMapper,
  ProjectType as ProjectTypeMapper,
  UserDefinition as UserDefinitionMapper,
  User as UserMapper,
  ComponentOffer as ComponentOfferMapper,
  Component as ComponentMapper,
  Provider as ProviderMapper,
  TeamCloudInstance as TeamCloudInstanceMapper
} from "../models/mappers";

export const accept: OperationParameter = {
  parameterPath: "accept",
  mapper: {
    defaultValue: "application/json",
    isConstant: true,
    serializedName: "Accept",
    type: {
      name: "String"
    }
  }
};

export const $host: OperationURLParameter = {
  parameterPath: "$host",
  mapper: {
    serializedName: "$host",
    required: true,
    type: {
      name: "String"
    }
  },
  skipEncoding: true
};

export const contentType: OperationParameter = {
  parameterPath: ["options", "contentType"],
  mapper: {
    defaultValue: "application/json",
    isConstant: true,
    serializedName: "Content-Type",
    type: {
      name: "String"
    }
  }
};

export const body: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ProjectDefinitionMapper
};

export const accept1: OperationParameter = {
  parameterPath: "accept",
  mapper: {
    defaultValue: "application/json",
    isConstant: true,
    serializedName: "Accept",
    type: {
      name: "String"
    }
  }
};

export const projectNameOrId: OperationURLParameter = {
  parameterPath: "projectNameOrId",
  mapper: {
    serializedName: "projectNameOrId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const projectId: OperationURLParameter = {
  parameterPath: "projectId",
  mapper: {
    serializedName: "projectId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body1: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ComponentRequestMapper
};

export const componentId: OperationURLParameter = {
  parameterPath: "componentId",
  mapper: {
    serializedName: "componentId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body2: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ProjectLinkMapper
};

export const linkId: OperationURLParameter = {
  parameterPath: "linkId",
  mapper: {
    serializedName: "linkId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const offerId: OperationURLParameter = {
  parameterPath: "offerId",
  mapper: {
    serializedName: "offerId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const includeShared: OperationQueryParameter = {
  parameterPath: ["options", "includeShared"],
  mapper: {
    serializedName: "includeShared",
    type: {
      name: "Boolean"
    }
  }
};

export const providerId: OperationURLParameter = {
  parameterPath: "providerId",
  mapper: {
    serializedName: "providerId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body3: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ProviderDataMapper
};

export const providerDataId: OperationURLParameter = {
  parameterPath: "providerDataId",
  mapper: {
    serializedName: "providerDataId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body4: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: {
    serializedName: "body",
    type: {
      name: "Dictionary",
      value: { type: { name: "String" } }
    }
  }
};

export const tagKey: OperationURLParameter = {
  parameterPath: "tagKey",
  mapper: {
    serializedName: "tagKey",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body5: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ProjectTypeMapper
};

export const projectTypeId: OperationURLParameter = {
  parameterPath: "projectTypeId",
  mapper: {
    serializedName: "projectTypeId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body6: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: UserDefinitionMapper
};

export const userNameOrId: OperationURLParameter = {
  parameterPath: "userNameOrId",
  mapper: {
    serializedName: "userNameOrId",
    required: true,
    type: {
      name: "String"
    }
  }
};

export const body7: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: UserMapper
};

export const body8: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ComponentOfferMapper
};

export const body9: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ComponentMapper
};

export const body10: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: ProviderMapper
};

export const trackingId: OperationURLParameter = {
  parameterPath: "trackingId",
  mapper: {
    serializedName: "trackingId",
    required: true,
    type: {
      name: "Uuid"
    }
  }
};

export const accept2: OperationParameter = {
  parameterPath: "accept",
  mapper: {
    defaultValue: "application/json, text/json",
    isConstant: true,
    serializedName: "Accept",
    type: {
      name: "String"
    }
  }
};

export const accept3: OperationParameter = {
  parameterPath: "accept",
  mapper: {
    defaultValue: "application/json, text/json",
    isConstant: true,
    serializedName: "Accept",
    type: {
      name: "String"
    }
  }
};

export const body11: OperationParameter = {
  parameterPath: ["options", "body"],
  mapper: TeamCloudInstanceMapper
};

export const userId: OperationURLParameter = {
  parameterPath: "userId",
  mapper: {
    serializedName: "userId",
    required: true,
    type: {
      name: "String"
    }
  }
};
