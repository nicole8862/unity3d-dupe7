using System;
using System.Collections.Generic;

namespace Dupe7
{
	struct IntegerRun
	{
		public int intgr;
		public int count;

		public static void From2DArray (int[,] ints, ref List<IntegerRun>[] runs)
		{
			int cols = ints.GetUpperBound (0) + 1, rows = ints.GetUpperBound (1) + 1;
			if (null == runs || runs.Length != rows) {
				runs = new List<IntegerRun>[rows];
				for (int r = 0; r < rows; r++)
					runs[r] = new List<IntegerRun> (cols);
			} else
				for (int r = 0; r < rows; r++)
					runs[r].Clear ();
			
			for (int r = 0; r < rows; r++) {
				IntegerRun run = new IntegerRun { intgr = ints[0, r], count = 1 };
				for (int c = 1; c < cols; c++)
					if (run.intgr != ints[c, r]) {
						runs[r].Add (run);
						run = new IntegerRun { intgr = ints[c, r], count = 1 };
					} else
						run.count++;
				runs[r].Add (run);
			}
		}
	}
}
