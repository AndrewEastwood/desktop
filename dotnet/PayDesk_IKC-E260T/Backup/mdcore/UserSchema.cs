using System;
using System.Collections.Generic;
using System.Text;

namespace mdcore
{
    [Serializable]
    public class UserSchema
    {
        private bool[] table;
        private string name;
        public bool[] SchemaTable { get { return table; } set { table = value; } }
        public string SchemaName { get { return name; } set { name = value; } }

        //-------------- Struct

        static private string[] items = new string[]{
            "������ ������ ���������",//0
            "��������� ������ ����",//1
            "��������� ���� ����",//2
            "������� ������/��������",//3
            "��������� ��� ������������ ��������",//4
            "���� ���������� ������",//5
            "���������� ���� ��� �������� ������",//6
            "��'�������� ������� ������",//7
            "������������� ������� ������ ����",//8
            "������������� ������ �� ���. �-�� ������",//9
            "���� ���������� ���� �� ���. ��������",//10
            "���� ������������ ���� �� ���. ��������",//11
            "���������� ���� ������������ ����",//12
            "������������ ������ ������� (999...)",//13
            "��������� ������� ������ �� ����",//14
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

        public string[] SchemaItems { get { return items; } }
        static public byte ItemsCount { get { return (byte)items.Length; } }
    }
}
