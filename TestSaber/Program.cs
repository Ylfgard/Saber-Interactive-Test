using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TestSaber
{
    internal class Program
    {
        public static void Main()
        {
            ListRand listTest = new ListRand(10);
            using (FileStream s = new FileStream(@"List save.txt",
                FileMode.Create, FileAccess.Write))
            {
                listTest.Serialize(s);
            }

            using (FileStream s = new FileStream(@"List save.txt",
                FileMode.Open, FileAccess.Read))
            {
                listTest.Deserialize(s);
            }
        }
    }

    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;

        public ListNode() {}

        public void Initialize(ListNode prev, ListNode next, ListNode rand, string data)
        {
            Prev = prev;
            Next = next;
            Rand = rand;
            Data = data;
        }

        public void Display()
        {
            string text = "";
            if(Prev != null) text = "Prev: " + Prev.Data + " ";
            text = text + "Data: " + Data + " ";
            if(Next != null) text = text + "Next: " + Next.Data + " ";
            Console.WriteLine(text + "Rand: " + Rand.Data);
        }
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public ListRand(int count)
        {
            Count = count;
            ListNode[] nodes = new ListNode[count];
            for(int i = 0; i < Count; i++)
            {
                ListNode node = new ListNode();
                nodes[i] = node;
            }

            Random r = new Random();
            ListNode prev;
            ListNode next;
            ListNode rnd;
            for(int i = 0; i < Count; i++)
            {
                if(i > 0) prev = nodes[i - 1];
                else prev = null;
                if(i < Count - 1) next = nodes[i + 1];
                else next = null;
                rnd = nodes[r.Next(0, Count)];

                nodes[i].Initialize(prev, next, rnd, i.ToString());
            }
            
            for(int i = 0; i < Count; i++)
                nodes[i].Display();

            Head = nodes[0];
            Tail = nodes[Count - 1];
        }

        public void Serialize(FileStream s)
        {
            byte[] number = BitConverter.GetBytes(Count);
            s.Write(number, 0, number.Length);

            byte[] text; ListNode cur = Head; //Сохраняю ListNode.Data
            Dictionary<ListNode, int> nodes = new Dictionary<ListNode, int>();
            for(int i = 0; i < Count; i++)
            {
                nodes.Add(cur, i);
                text = Encoding.Default.GetBytes(cur.Data);
                number = BitConverter.GetBytes(text.Length);
                s.Write(number, 0, number.Length);
                s.Write(text, 0, text.Length);

                cur = cur.Next;
            }

            cur = Head; int index; //Сохраняю индекс ListNode.Rand
            for (int i = 0; i < Count; i++)
            {
                nodes.TryGetValue(cur.Rand, out index);
                number = BitConverter.GetBytes(index);
                s.Write(number, 0, number.Length);
                
                cur = cur.Next;
            }
        }

        public void Deserialize(FileStream s)
        {
            byte[] number = new byte[4];
            s.Read(number, 0, number.Length);
            Count = BitConverter.ToInt32(number, 0);

            ListNode prev; ListNode next; int length;
            ListNode[] nodes = new ListNode[Count]; nodes[0] = new ListNode();
            for (int i = 0; i < Count; i++)
            {
                if (i > 0) prev = nodes[i - 1];
                else prev = null;
                if (i < Count - 1)
                {
                    nodes[i + 1] = new ListNode();
                    next = nodes[i + 1];
                }
                else next = null;
                nodes[i].Prev = prev;
                nodes[i].Next = next;

                s.Read(number, 0, number.Length);
                length = BitConverter.ToInt32(number, 0);
                byte[] text = new byte[length];
                s.Read(text, 0, length);
                string data = Encoding.Default.GetString(text);
                nodes[i].Data = data;
            }

            int index;
            for(int i = 0; i < Count; i++)
            {
                s.Read(number, 0, number.Length);
                index = BitConverter.ToInt32(number, 0);
                nodes[i].Rand = nodes[index];

                nodes[i].Display();
            }
            
            Head = nodes[0]; 
            Tail = nodes[Count - 1];
        }
    }
}