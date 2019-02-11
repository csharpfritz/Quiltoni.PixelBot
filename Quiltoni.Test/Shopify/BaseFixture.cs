using System.IO;

namespace Quiltoni.Test.Shopify
{
	public class BaseFixture {

		protected void ExtractTestFilesResource(string fileName, string targetFolder = "") {

			var targetFileName = fileName;
			if (!string.IsNullOrEmpty(targetFolder))
				targetFileName = Path.Combine(targetFolder, fileName);
			if (File.Exists(fileName)) return;

			var stream = GetType().Assembly.GetManifestResourceStream("Quiltoni.Test.Shopify." + fileName);
			var sw = new StreamWriter(targetFileName) {
				AutoFlush = true
			};
			var sr = new StreamReader(stream);

			sw.Write(sr.ReadToEnd());
			sw.Close();
			sw.Dispose();
			sr.Dispose();

		}

		protected string GetJsonFromResource(string fileName) {

			var stream = GetType().Assembly.GetManifestResourceStream("Quiltoni.Test.Shopify." + fileName);

			string outJson = string.Empty;

			using (var sr = new StreamReader(stream)) {

				outJson = sr.ReadToEnd();

			}

			return outJson;

		}

	}

}
