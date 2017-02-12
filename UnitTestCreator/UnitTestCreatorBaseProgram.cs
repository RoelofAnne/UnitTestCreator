using System;
using System.Collections.Generic;

namespace  UnitTestCreator
{
	public class  UnitTestCreatorBase
	{		
		/// <summary>
		/// Fetchs the details of the given project(s) location and creates the testfiles in testlocation
		/// mimicing the original locations of the project files
		/// </summary>
		/// <param name="original_map">Original code location</param>
		/// <param name="test_map">Test map location</param>
		public static void CreateUnitTests (string original_map, string test_map)
		{
			//Check if there a vb files in the original location
			var vb_files = System.IO.Directory.GetFiles (original_map, "*.vb", System.IO.SearchOption.AllDirectories);

			if (vb_files.Length > 0) {
				VbNunitTestCreator vb_nunit_creator = new VbNunitTestCreator();
				foreach (string file in vb_files) {
					if (!file.EndsWith (".designer.vb") && !file.EndsWith ("Test.vb")) {
						vb_nunit_creator.createTestFileFrom (file, test_map);
					}
				}
			}

			//Check if there a cs files in the original location
			var cs_files = System.IO.Directory.GetFiles (original_map, "*.cs", System.IO.SearchOption.AllDirectories);

			if (cs_files.Length > 0) {
				CsNunitTestCreator cs_nunit_creator = new CsNunitTestCreator();
				foreach (string file in vb_files) {
					if (!file.EndsWith (".designer.cs") && !file.EndsWith ("Test.cs")) {
						cs_nunit_creator.createTestFileFrom (file, test_map);
					}
				}
			}
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">"folder={solution}={test}"
		///  , where {solution} is the base of the solution tree and {test} is 
		///  the location where the tree of testfiles is placed.
		/// </param>
		public static void Main (string[] args)
		{
			if (args.GetLength (0) == 1) {
				if (args [0].StartsWith ("folder=")) {		
					String local_directory_and_testmainfolder = args [0].Substring (7);
				    
					String local_directory = local_directory_and_testmainfolder.Substring (0, local_directory_and_testmainfolder.IndexOf ("="));
					String test_maindirectory = local_directory_and_testmainfolder.Substring (local_directory_and_testmainfolder.IndexOf ("=") + 1);
					if (System.IO.Directory.Exists (local_directory)) {
						System.IO.Directory.CreateDirectory (test_maindirectory);
						CreateUnitTests (local_directory, test_maindirectory);
					} else {
						Console.WriteLine ("Next time, Next time,...");
					}
				} else {
					Console.WriteLine ("Next time, Next time,...");
				}
			} else {
				Console.WriteLine ("Next time, Next time,...");
			}
		}
	}
}
