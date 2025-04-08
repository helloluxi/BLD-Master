using System;
using System.IO;
using static CS.Cube555.Util;
namespace CS.Cube555;

public class Tools {

	public static bool SaveToFile(string filename, int[] obj) {
		try {
            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            using BinaryWriter writer = new(File.Open(filename, FileMode.Create));
            writer.Write(obj.Length);
            byte[] buffer = new byte[obj.Length * sizeof(int)];
            Buffer.BlockCopy(obj, 0, buffer, 0, buffer.Length);
            writer.Write(buffer);
            return true;
		} catch {
			return false;
		}
	}

	public static int[] LoadFromFile(string filename) {
		try {
            using BinaryReader reader = new(File.Open(filename, FileMode.Open));
            int length = reader.ReadInt32();
            byte[] buffer = reader.ReadBytes(length * sizeof(int));
            int[] ret = new int[length];
            Buffer.BlockCopy(buffer, 0, ret, 0, buffer.Length);
            return ret;
        } catch {
			return null;
		}
	}

	static CubieCube randomCubieCube(Random gen) {
		CubieCube cc = new();
		for(int i = 0; i < 23; i++) {
			swap(cc.xCenter, i, i + gen.Next(24 - i));
			swap(cc.tCenter, i, i + gen.Next(24 - i));
			swap(cc.wEdge, i, i + gen.Next(24 - i));
		}
		int eoSum = 0;
		int eParity = 0;
		for(int i = 0; i < 11; i++) {
			int swap_ = gen.Next(12 - i);
			if (swap_ != 0) {
				swap(cc.mEdge, i, i + swap_);
				eParity ^= 1;
			}
			int flip = gen.Next(2);
			cc.mEdge[i] ^= flip;
			eoSum ^= flip;
		}
		cc.mEdge[11] ^= eoSum;
        int cp;
        do {
			cp = gen.Next(40320);
		} while (eParity != getParity(cp, 8));
		cc.corner.copy(new CubieCube.CornerCube(cp, gen.Next(2187)));
		return cc;
	}

	public static string randomCube(Random gen) {
		return randomCubieCube(gen).toFacelet();
	}
}