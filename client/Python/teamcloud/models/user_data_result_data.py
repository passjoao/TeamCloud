# coding=utf-8
# --------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator.
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.
# --------------------------------------------------------------------------

from .user import User


class UserDataResultData(User):
    """UserDataResultData.

    :param id:
    :type id: str
    :param role:
    :type role: str
    :param tags:
    :type tags: dict[str, str]
    """

    _attribute_map = {
        'id': {'key': 'id', 'type': 'str'},
        'role': {'key': 'role', 'type': 'str'},
        'tags': {'key': 'tags', 'type': '{str}'},
    }

    def __init__(self, **kwargs):
        super(UserDataResultData, self).__init__(**kwargs)