using System;
using System.Collections.Generic;

namespace  UnitTestCreator
{
	public class  VbNunitTestCreator
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
				temp.Add ("Option Explicit On");
				temp.Add ("Imports NUnit.Framework");
				temp.Add ("");

				//loop through the content to analyse
				foreach (String line in filecontent) {
					if (line.Contains ("Namespace") && !line.Contains ("End")) {
						temp.Add (line);
						isNamespace = true;
					} else if (line.Contains ("Class") && !line.Contains ("End")) {
						temp.Add ("<TestFixture()>");
						class_name = line.Replace ("Private", "Protected").Replace ("Protected", "Public").Replace ("Public Class", "");
						temp.Add (line.Replace("Private","Protected").Replace("Protected","Public") + "Test");
						temp.Add (" Inherits AssertionHelper");
						temp.Add ("");
						temp.Add (" <SetUp()> _");
						temp.Add (" Protected Sub SetUp()");
						temp.Add ("  'This sub can be used to setup variables used in Nunit tests");
						temp.Add (" End Sub");
						temp.Add ("");
						temp.Add (" <TearDown()> _");
						temp.Add (" Protected Sub TearDown()");
						temp.Add ("  'This sub can be used to TearDown the setuped variables used in Nunit tests");
						temp.Add (" End Sub");
						isclass = true;
					} else if (line.Contains ("Sub") && !line.Contains ("End")) {
						temp.Add ("");
						temp.Add (" 'Maybe you can think of a test for this sub?");
						temp.Add (" 'Or rewrite to a function call and test that function?");
						temp.Add (" <Test()>");
						string sub_name = line.Substring (0, line.IndexOf ("("));
						sub_name = sub_name.Substring (sub_name.LastIndexOf (" "));
						temp.Add ("Public Sub "+sub_name+"Test()");
						temp.Add ("  'A few lines of help follow");
						temp.Add ("  '' Classic syntax");
						temp.Add ("  'Assert.IsTrue(2 + 2 = 4)");
						temp.Add ("  '' Helper syntax");
						temp.Add ("  'Assert.That(2 + 2 = 4, [Is].True)");
						temp.Add ("  'Assert.That(2 + 2 = 4)");
						temp.Add ("  ''Inherited syntax");
						temp.Add ("  'Expect(2 + 2 = 4, True)");
						temp.Add ("  'Expect(2 + 2 = 4)");
						temp.Add ("");
						temp.Add ("  'To indicate that this test is not yet valid");
						temp.Add ("  Assert.IsTrue(False)");
						temp.Add ("  'Actual parameters of mentioned sub are: " + line.Replace (line.Substring (0, line.IndexOf ("(")), "").Replace(")",""));
						temp.Add (" End Sub");
					} else if (line.Contains ("Function") && !line.Contains ("End")) {
						temp.Add ("");
						temp.Add (" <Test()>");
						string function_name = line.Substring (0, line.IndexOf ("("));
						function_name = function_name.Substring (function_name.LastIndexOf (" "));
						temp.Add ("Public Sub "+function_name+"Test()");
						temp.Add ("  'You might want to use the class object to access the methods");
						temp.Add ("  Dim test"+class_name+" As " + class_name + " = new " + class_name + "()");

						// If there are parameters, then add them
						string parameters = "";
						if (!line.Contains("()"))
						{
							char[] splitchar = {',',')'};
							var list_of_parameters = line.Replace (line.Substring (0, line.IndexOf ("(")+1), "").Split (splitchar);
							foreach (string parameter in list_of_parameters)
							{
								string param = parameter.Trim();
								if (!param.StartsWith("As"))
								{

									// Set default expected
									if (param.EndsWith("As String"))
									{
										temp.Add("  Dim "+param.Replace("ByVal","").Replace("ByRef","") +" = String.Empty");
									}
									else if (param.EndsWith("As Bool"))
									{
										temp.Add("  Dim "+param.Replace("ByVal","").Replace("ByRef","") + " = False");
									}// Other types may be extended later
									else
									{
										temp.Add("  Dim "+param.Replace("ByVal","").Replace("ByRef","") + " = Nothing");
									}

									parameters = parameters + "," +param.Substring (0, param.IndexOf (" As")).Replace("ByVal","").Replace("ByRef","");
								}
							}
							if (parameters.Length>0)
							{
								parameters = parameters.Substring(1);
							}
						}

						// Set default expected
						if (line.EndsWith("As String"))
						{
							temp.Add ("  Dim expected As String = String.Empty");
						}
						else if (line.EndsWith("As Bool"))
						{
							temp.Add ("  Dim expected As Bool = False");
						}// Other types may be extended later
						else
						{
							temp.Add ("  Dim expected = Nothing");
						}

						temp.Add ("  Assert.AreEqual(expected, test"+class_name+"." + function_name + "("+parameters+"))");

						temp.Add (" End Sub");
					}

				}

				if (isclass) {
					temp.Add ("End Class");
				}
				if (isNamespace) {
					temp.Add ("End Namespace");
				}

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
			String test_file_location = testmap + stripped_filename_and_special_path.Replace (".vb", "Test.vb");

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
