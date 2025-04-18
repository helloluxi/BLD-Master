﻿using System.Text;
namespace Cube;
public class Alg : List<Move>
{
    public Alg() { }
    public Alg(IEnumerable<Move> moves) : base(moves) { }
    public Alg(string s)
    {
        Queue<int> q3 = new(), q4 = new();
        foreach (char c in s)
            switch (c)
            {
                case 'U': Add(Move.U); break;
                case 'u': Add(Move.u); break;
                case 'D': Add(Move.D); break;
                case 'd': Add(Move.d); break;
                case 'R': Add(Move.R); break;
                case 'r': Add(Move.r); break;
                case 'L': Add(Move.L); break;
                case 'l': Add(Move.l); break;
                case 'F': Add(Move.F); break;
                case 'f': Add(Move.f); break;
                case 'B': Add(Move.B); break;
                case 'b': Add(Move.b); break;
                case 'E': Add(Move.E); break;
                case 'y': Add(Move.y); break;
                case 'M': Add(Move.M); break;
                case 'x': Add(Move.x); break;
                case 'S': Add(Move.S); break;
                case 'z': Add(Move.z); break;
                case '3': q3.Enqueue(Count); break;
                case '4': q4.Enqueue(Count); break;
                default:
                    if (Count == 0) break;
                    int i = (int)this[Count - 1];
                    if (c == '2') this[Count - 1] = (Move)(i / 3 * 3 + 1);
                    else if (c == '\'' && i % 3 == 0) this[Count - 1] = (Move)(i + 2);
                    else if (c == 'w' && i % 18 == 0) this[Count - 1] = (Move)(i + 6);
                    break;
            }
        foreach (var i in q3)
            if (i < Count && ((int)this[i]) % 18 / 3 == 2)
                this[i] = (Move)((int)this[i] + 3);
        foreach (var i in q4)
            if (i < Count && ((int)this[i]) % 18 / 3 == 2)
                this[i] = (Move)((int)this[i] + 6);
    }
    // Operator +
    public static Alg operator +(Alg a, Alg b) => [.. a, .. b];
    public static Alg Parse(string s){ // Broken but enough for me~
        if (s.Contains(':')) {
            int colonIdx = s.IndexOf(':');
            var pre = Parse(s[..colonIdx]);
            var core = Parse(s[(colonIdx+1)..]);
            return pre + core + pre.GetInv();
        }
        else if (s.Contains('[')) { // No support of stack
            int commaIdx = s.IndexOf(',');
            var a = Parse(s[(s.IndexOf('[')+1)..commaIdx]);
            var b = Parse(s[(commaIdx+1)..s.LastIndexOf(']')]);
            return a + b + a.GetInv() + b.GetInv();
        }
        else if (s.Contains(")2")){
            var a = Parse(s[(s.IndexOf('(')+1)..s.LastIndexOf(")2")]);
            return a + a;
        }
        else return new Alg(s);
    }
    public static Move Inv(Move m){
        return (Move)((int)m + ((int)m % 3 == 0 ? 2 : (int)m % 3 == 2 ? -2 : 0));
    }
    public Alg this[Range range] => [.. this.Skip(range.Start.GetOffset(Count)).Take(range.End.GetOffset(Count) - range.Start.GetOffset(Count))];
    public Alg GetInv(){
        Alg alg = [];
        for (int i = Count - 1; i >= 0; i--)
            alg.Add(Inv(this[i]));
        return alg;
    }
    public override string ToString()
    {
        string[] strings = new string[Count];
        for (int i = 0; i < Count; i++)
            strings[i] = this[i].ToString().TrimStart('_').Replace('_', '\'');
        return string.Join(" ", strings);
    }
    public Alg Shift(int n){
        Alg alg = [];
        for (int i = 0; i < Count; i++)
            alg.Add(this[(i + n + Count) % Count]);
        return alg;
    }
    public IEnumerable<Alg> Shifts(){
        for (int i = 0; i < Count; i++)
            yield return Shift(i);
    }
    public IEnumerable<(Alg, Alg)> Cuts(){
        for (int i = 0; i <= Count; i++)
            yield return (this[..i], this[i..]);
    }
}

