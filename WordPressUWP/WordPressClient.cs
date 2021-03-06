﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WordPressUWP.Models;

namespace WordPressUWP
{
	public class WordPressClient
	{
		private readonly string _endpoint;

		public WordPressClient(string endpoint)
		{
			if (string.IsNullOrWhiteSpace(endpoint))
			{
				throw new ArgumentNullException(nameof(endpoint));
			}

			if (!endpoint.EndsWith("/"))
			{
				endpoint += "/";
			}

			_endpoint = endpoint;
		}

		public string Endpoint
		{
			get { return _endpoint; }
		}

		public async Task<IList<Post>> ListPosts(int page = 1, int per_page = 10, int offset = 0, Post.OrderBy orderby = Post.OrderBy.date)
		{
			return await Download<Post[]>($"posts").ConfigureAwait(false);
		}

		public async Task<Post> GetPost(String id)
		{
			return await Download<Post>($"posts/{id}").ConfigureAwait(false);
		}

		public async Task<IList<Comment>> ListComments()
		{
			return await Download<Comment[]>("comments").ConfigureAwait(false);
		}

		public async Task<Comment> GetComment(string id)
		{
			return await Download<Comment>($"comment/{id}").ConfigureAwait(false);
		}

		protected async Task<TClass> Download<TClass>(string section)
			where TClass : class
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync($"{Endpoint}/{section}").ConfigureAwait(false);
				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					return JsonConvert.DeserializeObject<TClass>(responseString);
				}
			}
			return default(TClass);
		}
	}
}