using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BeerMP.Networking
{
	// Token: 0x02000049 RID: 73
	public class Packet : IDisposable
	{
		// Token: 0x0600016D RID: 365 RVA: 0x0000781B File Offset: 0x00005A1B
		public Packet()
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
			this.id = 0;
			this.scene = 1;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00007843 File Offset: 0x00005A43
		public Packet(int _id)
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
			this.id = _id;
			this.scene = 1;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000786B File Offset: 0x00005A6B
		internal Packet(int _id, GameScene _scene = GameScene.GAME)
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
			this.id = _id;
			this.scene = (int)_scene;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00007893 File Offset: 0x00005A93
		internal Packet(byte[] _data)
		{
			this.buffer = new List<byte>();
			this.readPos = 0;
			this.SetBytes(_data);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000078B4 File Offset: 0x00005AB4
		public void SetBytes(byte[] _data)
		{
			this.Write(_data, -1);
			this.readableBuffer = this.buffer.ToArray();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000078CF File Offset: 0x00005ACF
		internal void WriteLength()
		{
			this.buffer.InsertRange(0, BitConverter.GetBytes(this.buffer.Count));
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000078ED File Offset: 0x00005AED
		internal void InsertString(string _value)
		{
			this.buffer.InsertRange(0, Encoding.ASCII.GetBytes(_value));
			this.buffer.InsertRange(0, BitConverter.GetBytes(_value.Length));
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000791D File Offset: 0x00005B1D
		public byte[] ToArray()
		{
			this.readableBuffer = this.buffer.ToArray();
			return this.readableBuffer;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00007936 File Offset: 0x00005B36
		public int Length()
		{
			return this.buffer.Count;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00007943 File Offset: 0x00005B43
		public int UnreadLength()
		{
			return this.Length() - this.readPos;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00007952 File Offset: 0x00005B52
		public void Reset(bool _shouldReset = true)
		{
			if (_shouldReset)
			{
				this.buffer.Clear();
				this.readableBuffer = null;
				this.readPos = 0;
				return;
			}
			this.readPos -= 4;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000797F File Offset: 0x00005B7F
		public void Write(byte _value, int index = -1)
		{
			if (index < 0 || index > this.buffer.Count)
			{
				this.buffer.Add(_value);
				return;
			}
			this.buffer.Insert(index, _value);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x000079AD File Offset: 0x00005BAD
		public void Write(byte[] _value, int index = -1)
		{
			if (index < 0 || index > this.buffer.Count)
			{
				this.buffer.AddRange(_value);
				return;
			}
			this.buffer.InsertRange(index, _value);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000079DB File Offset: 0x00005BDB
		public void Write(short _value, int index = -1)
		{
			this.Write(BitConverter.GetBytes(_value), index);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000079EA File Offset: 0x00005BEA
		public void Write(int _value, int index = -1)
		{
			this.Write(BitConverter.GetBytes(_value), index);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x000079F9 File Offset: 0x00005BF9
		public void Write(long _value, int index = -1)
		{
			this.Write(BitConverter.GetBytes(_value), index);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00007A08 File Offset: 0x00005C08
		public void Write(float _value, int index = -1)
		{
			this.Write(BitConverter.GetBytes(_value), index);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00007A17 File Offset: 0x00005C17
		public void Write(bool _value, int index = -1)
		{
			this.Write(BitConverter.GetBytes(_value), index);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00007A26 File Offset: 0x00005C26
		public void Write(string _value, int index = -1)
		{
			this.Write(_value.Length, -1);
			this.Write(Encoding.ASCII.GetBytes(_value), index);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00007A47 File Offset: 0x00005C47
		public void Write(Vector2 _value, int index = -1)
		{
			this.Write(_value.x, index);
			this.Write(_value.y, index);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00007A63 File Offset: 0x00005C63
		public void Write(Vector3 _value, int index = -1)
		{
			this.Write(_value.x, index);
			this.Write(_value.y, index);
			this.Write(_value.z, index);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00007A8C File Offset: 0x00005C8C
		public void Write(Vector4 _value, int index = -1)
		{
			this.Write(_value.x, index);
			this.Write(_value.y, index);
			this.Write(_value.z, index);
			this.Write(_value.w, index);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00007AC2 File Offset: 0x00005CC2
		public void Write(Quaternion _value, int index = -1)
		{
			this.Write(_value.x, index);
			this.Write(_value.y, index);
			this.Write(_value.z, index);
			this.Write(_value.w, index);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00007AF8 File Offset: 0x00005CF8
		public byte ReadByte(bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				byte b = this.readableBuffer[this.readPos];
				if (_moveReadPos)
				{
					this.readPos++;
				}
				return b;
			}
			throw new Exception("Could not read value of type 'byte'!");
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00007B38 File Offset: 0x00005D38
		public byte[] ReadBytes(int _length, bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				byte[] array = this.buffer.GetRange(this.readPos, _length).ToArray();
				if (_moveReadPos)
				{
					this.readPos += _length;
				}
				return array;
			}
			throw new Exception("Could not read value of type 'byte[]'!");
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00007B8C File Offset: 0x00005D8C
		public short ReadShort(bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				short num = BitConverter.ToInt16(this.readableBuffer, this.readPos);
				if (_moveReadPos)
				{
					this.readPos += 2;
				}
				return num;
			}
			throw new Exception("Could not read value of type 'short'!");
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00007BDC File Offset: 0x00005DDC
		public int ReadInt(bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				int num = BitConverter.ToInt32(this.readableBuffer, this.readPos);
				if (_moveReadPos)
				{
					this.readPos += 4;
				}
				return num;
			}
			throw new Exception("Could not read value of type 'int'!");
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00007C2C File Offset: 0x00005E2C
		public long ReadLong(bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				long num = BitConverter.ToInt64(this.readableBuffer, this.readPos);
				if (_moveReadPos)
				{
					this.readPos += 8;
				}
				return num;
			}
			throw new Exception("Could not read value of type 'long'!");
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00007C7C File Offset: 0x00005E7C
		public float ReadFloat(bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				float num = BitConverter.ToSingle(this.readableBuffer, this.readPos);
				if (_moveReadPos)
				{
					this.readPos += 4;
				}
				return num;
			}
			throw new Exception("Could not read value of type 'float'!");
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00007CCC File Offset: 0x00005ECC
		public bool ReadBool(bool _moveReadPos = true)
		{
			if (this.buffer.Count > this.readPos)
			{
				bool flag = BitConverter.ToBoolean(this.readableBuffer, this.readPos);
				if (_moveReadPos)
				{
					this.readPos++;
				}
				return flag;
			}
			throw new Exception("Could not read value of type 'bool'!");
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00007D1C File Offset: 0x00005F1C
		public string ReadString(bool _moveReadPos = true)
		{
			string text;
			try
			{
				int num = this.ReadInt(true);
				string @string = Encoding.ASCII.GetString(this.readableBuffer, this.readPos, num);
				if (_moveReadPos && @string.Length > 0)
				{
					this.readPos += num;
				}
				text = @string;
			}
			catch
			{
				throw new Exception("Could not read value of type 'string'!");
			}
			return text;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00007D84 File Offset: 0x00005F84
		public Vector2 ReadVector2(bool _moveReadPos = true)
		{
			int num = this.readPos;
			Vector2 vector = new Vector2(this.ReadFloat(true), this.ReadFloat(true));
			if (!_moveReadPos)
			{
				this.readPos = num;
			}
			return vector;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00007DB8 File Offset: 0x00005FB8
		public Vector3 ReadVector3(bool _moveReadPos = true)
		{
			int num = this.readPos;
			Vector3 vector = new Vector3(this.ReadFloat(true), this.ReadFloat(true), this.ReadFloat(true));
			if (!_moveReadPos)
			{
				this.readPos = num;
			}
			return vector;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00007DF0 File Offset: 0x00005FF0
		public Vector4 ReadVector4(bool _moveReadPos = true)
		{
			int num = this.readPos;
			Vector4 vector = new Vector4(this.ReadFloat(true), this.ReadFloat(true), this.ReadFloat(true), this.ReadFloat(true));
			if (!_moveReadPos)
			{
				this.readPos = num;
			}
			return vector;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00007E30 File Offset: 0x00006030
		public Quaternion ReadQuaternion(bool _moveReadPos = true)
		{
			int num = this.readPos;
			Quaternion quaternion = new Quaternion(this.ReadFloat(true), this.ReadFloat(true), this.ReadFloat(true), this.ReadFloat(true));
			if (!_moveReadPos)
			{
				this.readPos = num;
			}
			return quaternion;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00007E6F File Offset: 0x0000606F
		protected virtual void Dispose(bool _disposing)
		{
			if (!this.disposed)
			{
				if (_disposing)
				{
					this.buffer = null;
					this.readableBuffer = null;
					this.readPos = 0;
				}
				this.disposed = true;
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00007E98 File Offset: 0x00006098
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000141 RID: 321
		private List<byte> buffer;

		// Token: 0x04000142 RID: 322
		private byte[] readableBuffer;

		// Token: 0x04000143 RID: 323
		private int readPos;

		// Token: 0x04000144 RID: 324
		internal int id;

		// Token: 0x04000145 RID: 325
		internal int scene;

		// Token: 0x04000146 RID: 326
		private bool disposed;
	}
}
