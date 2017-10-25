using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public static class AppConfig
	{
		public static Uri ElasticsearchUri => new Uri(ElasticsearchUrl);
		public static string ElasticsearchUrl => "";
		public static string ElasticsearchUsername => "";
		public static string ElasticsearchPassword => "";
	}
}
