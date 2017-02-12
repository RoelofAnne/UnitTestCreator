using System;
using System.Collections.Generic;

namespace  UnitTestCreator
{
	public class  CsNunitTestCreator
	{
		/// <summary>
		/// Anaylyses the code file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="filecontent">Filecontent.</param>
		private List<string> anaylyseFile (string[] filecontent)
		{
			bool isNamespace = false;
			bool isclass = false;
			string class_name = "";

			List<string> temp = new List<string> ();
			if (!filecontent[0].Contains("Generated Nunit test file, please adjust to your need.")) {
				temp.Add ("'Generated unit test file, please adjust to your need.");
				temp.Add ("'Copyright Roelof Anne Schoenmaker");
				temp.Add ("");

				// To be filled
			}
			return temp;
		}

		/// <summary>
		/// Check how much of the original and test map differ from each other
		/// </summary>
		/// <returns>The part that is equal between the original and test map</returns>
		/// <param name="dir">Dir.</param>
		/// <param name="test_map">Test map.</param>
		private string samePart (string dir, string test_map)
		{
			string samefromstart = "";
			int locator = 0;
			if (dir.Length < test_map.Length) {
				int length = dir.Length - 1;
				while (dir[locator]==test_map[locator] && locator < length) {
					locator++;
				}
				if (locator > 0) {
					samefromstart = dir.Substring (0, locator);
				}
			} else {
				int length = test_map.Length - 1;
				while (dir[locator]==test_map[locator] && locator < length) {
					locator++;
				}
				if (locator > 0) {
					samefromstart = test_map.Substring (0, locator);
				}
			}

			return samefromstart;
		}

		/// <summary>
		/// Creates the test file from the file in filepath and creates a testfile in the testmap.
		/// </summary>
		/// <param name="filepath">Filepath.</param>
		/// <param name="testmap">Testmap.</param>
		public void createTestFileFrom (string filepath, string testmap)
		{
			var file_content = System.IO.File.ReadAllLines (filepath);

			String samepath = samePart (filepath, testmap);
			String stripped_filename_and_special_path = filepath.Replace (samepath, "");
			String test_file_location = testmap + stripped_filename_and_special_path.Replace (".cs", "Test.cs");

			if (test_file_location.Contains ("/")) {
				if (!System.IO.Directory.Exists (test_file_location.Substring (0, test_file_location.LastIndexOf ("/")))) {
					System.IO.Directory.CreateDirectory (test_file_location.Substring (0, test_file_location.LastIndexOf ("/")));
				}
			} else {
				if (!System.IO.Directory.Exists (test_file_location.Substring (0, test_file_location.LastIndexOf ("\\")))) {
					System.IO.Directory.CreateDirectory (test_file_location.Substring (0, test_file_location.LastIndexOf ("\\")));
				}
			}

			// Only once a testfile is made, because of user changes
			if (!System.IO.File.Exists (test_file_location)) {
				System.IO.File.AppendAllLines (test_file_location, anaylyseFile (file_content));
			}

		}

	}
}
