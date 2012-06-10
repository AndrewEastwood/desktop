using System;
using System.Collections.Generic;
using System.Text;

namespace mdcore.Components
{
    [Serializable]
    public class UserSchema
    {
        // Private fields
        private bool[] _table;
        private string _name;
        private string[] _items = new string[]{
            "������ ������ ���������",//0
            "��������� ������ ����",//1
            "��������� ���� ����",//2
            "������� ������/��������",//3
            "��������� ��� ����������� ��������",//4
            "���� ���������� ������",//5
            "���������� ���� ��� �������� ������",//6
            "��'�������� ������� ������",//7
            "������������� ������� ������ ����",//8
            "������������� ������ �� ���. �-�� ������",//9
            "���� ���������� ���� �� ���. ��������",//10
            "���� ������������ ���� �� ���. ��������",//11
            "���������� ���� ������������ ����",//12
            "������������ ������ ������� (999...)",//13
            "��������� ������ ������ �� ����",//14
            "������������ ������ �볺��� (998...)",//15
            "������������ ���������� �����-����",//16
            "����� ��������� �-��",//17
            "������ �������",//18
            "������ � ������",//19
            "������ �����",//20
            "������ �������� �������",//21
            "������� �������",//22
            "��������� �������� ����",//23
            "��������� ���� ����"//24
        };

        // Public Constants
        public const byte ITEMS_COUNT = 25;

        // Properties
        public bool[] SchemaTable { get { return _table; } set { _table = value; } }
        public string SchemaName { get { return _name; } set { _name = value; } }
        public string[] SchemaItems { get { return _items; } }
    }
}
