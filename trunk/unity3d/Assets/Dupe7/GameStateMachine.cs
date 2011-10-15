using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dupe7
{
	public enum TurnState
	{
		Undefined,
		WaitingForPlayer,
		TurnStarted,
		Dropped,
		Marked,
		LevelUp,
		TurnEnded
	}

	[Serializable()]
	public class GameStateMachine : ISerializable
	{
		public static readonly int[] ChainScores = { 7, 39, 109, 224, 391, 617, 907, 2167, 1707, 2213 };

		public const int COLS = 7;
		public const int ROWS = 8;
		readonly int MaxTurns;
		readonly int MinTurns;
		uint randomSeed;
		public Disc[,] discs = new Disc[COLS, ROWS];
		public int[] columnCounts = new int[COLS];
		public DiscMarkedState[,] markStates = new DiscMarkedState[COLS, ROWS];
		int[,] occupiedInt = new int[COLS, ROWS];
		List<IntegerRun>[] rowRuns;

		#region Public Properties

		public TurnState Current { get; private set; }

		public Disc[,] Discs {
			get { return discs; }
		}

		public int[] ColumnCounts {
			get { return columnCounts; }
		}

		public Disc NextDisc { get; private set; }

		public int DropColumn { get; set; }

		public DiscMarkedState[,] DiscMarkedStates {
			get { return markStates; }
		}

		public int MarkedCount { get; private set; }

		public int Chain { get; private set; }

		public int ChainScore {
			get { return ChainScores[Chain < ChainScores.Length ? Chain : ChainScores.Length - 1]; }
		}

		public int Level { get; private set; }

		public int Score { get; private set; }

		public int NumTurnsThisLevel { get; private set; }

		public int MaxTurnsThisLevel { get; private set; }

		#endregion

		public GameStateMachine (int maxTurns, int minTurns, uint seed)
		{
			MaxTurns = maxTurns;
			MinTurns = minTurns;
			NumTurnsThisLevel = MaxTurnsThisLevel = MaxTurns;
			
			randomSeed = seed;
		}

		public bool MoveNext ()
		{
			// Change
			switch (Current) {
			
			case TurnState.Undefined:
				Current = TurnState.TurnEnded;
				break;
			
			case TurnState.WaitingForPlayer:
				if (DropColumn >= 0 && DropColumn < COLS && columnCounts[DropColumn] < ROWS - 1)
					Current = TurnState.TurnStarted;
				break;
			
			case TurnState.TurnStarted:
			case TurnState.Marked:
			case TurnState.LevelUp:
				Current = TurnState.Dropped;
				break;
			
			case TurnState.Dropped:
				if (0 == MarkedCount) {
					if (0 == NumTurnsThisLevel)
						Current = TurnState.LevelUp;
					else
						Current = TurnState.TurnEnded;
				} else
					Current = TurnState.Marked;
				break;
			
			case TurnState.TurnEnded:
				Current = TurnState.WaitingForPlayer;
				break;
			}
			
			// Action
			switch (Current) {
			
			case TurnState.TurnStarted:
				discs[DropColumn, columnCounts[DropColumn]++] = NextDisc;
				Chain = 0;
				NumTurnsThisLevel--;
				break;
			
			case TurnState.Dropped:
				if (MarkedCount > 0) {
					RemoveMarkedDiscs ();
					Chain++;
				}
				MarkDiscs ();
				break;
			
			case TurnState.Marked:
				if (MarkedCount > 0)
					Score += MarkedCount * ChainScores[Chain];
				break;
			
			case TurnState.LevelUp:
				for (int c = 0; c < COLS; c++) {
					for (int r = columnCounts[c]; r > 0; r--)
						discs[c, r] = discs[c, r - 1];
					
					discs[c, 0] = RandomDisc (true);
					columnCounts[c]++;
				}
				
				Score += 17000;
				Level++;
				NumTurnsThisLevel = MaxTurnsThisLevel;
				break;
			
			case TurnState.TurnEnded:
				NextDisc = RandomDisc (false);
				break;
			}
			
			return true;
		}
		
		public bool TestGameOver ()
		{
			bool one8 = false, all7 = true;
			for (int c = 0; c < COLS; c++) {
				one8 = one8 || (ROWS == columnCounts[c]);
				all7 = all7 && (ROWS - 1 == columnCounts[c]);
			}
			return (all7 || one8);
		}

		void MarkDiscs ()
		{
			for (int c = 0; c < COLS; c++)
				for (int r = 0; r < ROWS; r++)
					occupiedInt[c, r] = r < columnCounts[c] ? 1 : 0;
			
			IntegerRun.From2DArray (occupiedInt, ref rowRuns);
			
			Array.Clear (markStates, 0, markStates.Length);
			MarkedCount = MarkColumns () + MarkRows ();
			
			if (MarkedCount > 0)
				MarkNeighbors ();
		}
		
		void RemoveMarkedDiscs ()
		{
			for (int c = 0; c < COLS; c++) {
				int di = 0, cc = columnCounts[c];
				for (int si = 0; si < cc;) {
					while (si < cc && DiscMarkedState.DiscMarked == markStates[c, si])
						si++;
					if (si < cc) {
						discs[c, di] = discs[c, si];
						di++;
						si++;
					}
				}
				columnCounts[c] = di;
			}
		}

		int MarkColumns ()
		{
			// Clear stacks
			int markedCount = 0;
			for (int c = 0; c < COLS; c++) {
				bool cellClearedInStack = false;
				int count = columnCounts[c];
				for (int r = 0; r < count; r++) {
					if (DiscState.Open == discs[c, r].state && count == discs[c, r].num) {
						markStates[c, r] = DiscMarkedState.DiscMarked;
						markedCount++;
						cellClearedInStack = true;
					}
				}
				
				// Flag this stack as cleared
				if (cellClearedInStack)
					for (int r = 0; r < count; r++)
						if (DiscMarkedState.DiscMarked != markStates[c, r])
							markStates[c, r] = DiscMarkedState.StripMarked;
			}
			return markedCount;
		}

		int MarkRows ()
		{
			// Clear runs
			int markedCount = 0;
			for (int r = 0; r < ROWS; r++) {
				int c = 0;
				foreach (IntegerRun rowRun in rowRuns[r]) {
					int count = rowRun.count;
					if (0 != rowRun.intgr) {
						bool cellClearedInRun = false;
						for (int i = c; i < c + count; i++) {
							if (DiscState.Open == discs[i, r].state && count == discs[i, r].num) {
								markStates[i, r] = DiscMarkedState.DiscMarked;
								markedCount++;
								cellClearedInRun = true;
							}
						}
						
						if (cellClearedInRun)
							for (int i = c; i < c + count; i++)
								if (DiscMarkedState.DiscMarked != markStates[i, r])
									markStates[i, r] = DiscMarkedState.StripMarked;
					}
					c += count;
				}
			}
			return markedCount;
		}

		void MarkNeighbors ()
		{
			for (int c = 0; c < COLS; c++) {
				for (int r = 0, count = columnCounts[c]; r < count; r++) {
					bool lt = (c > 0) && (DiscMarkedState.DiscMarked == markStates[c - 1, r]);
					bool rt = (c < COLS - 1) && (DiscMarkedState.DiscMarked == markStates[c + 1, r]);
					bool up = (r > 0) && (DiscMarkedState.DiscMarked == markStates[c, r - 1]);
					bool dn = (r < count - 1) && (DiscMarkedState.DiscMarked == markStates[c, r + 1]);
					if ((lt || rt || up || dn) && discs[c, r].state > DiscState.Open)
						discs[c, r].state--;
				}
			}
		}

		Disc RandomDisc (bool startShut)
		{
			randomSeed = LCGRand.Next (randomSeed);
			int discNum = (int)((randomSeed % 7) + 1);
			return new Disc { num = discNum, state = startShut ? DiscState.Shut : DiscState.Open };
		}

		#region Serialization

		GameStateMachine (SerializationInfo info, StreamingContext unused)
		{
			MinTurns = info.GetInt32 ("MinTurns");
			MaxTurns = info.GetInt32 ("MaxTurns");
			MaxTurnsThisLevel = info.GetInt32 ("maxTurnsThisLevel");
			NumTurnsThisLevel = info.GetInt32 ("numTurnsThisLevel");
		}

		public void GetObjectData (SerializationInfo info, StreamingContext unused)
		{
			info.AddValue ("MinTurns", MinTurns);
			info.AddValue ("MaxTurns", MaxTurns);
			info.AddValue ("maxTurnsThisLevel", MaxTurnsThisLevel);
			info.AddValue ("numTurnsThisLevel", NumTurnsThisLevel);
		}
		
		#endregion
	}
}
