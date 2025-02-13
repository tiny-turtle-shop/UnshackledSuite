﻿namespace Unshackled.Fitness.My.Client.Services;

public interface ILocalStorage
{
	Task RemoveItemAsync(string key);
	Task<TOut?> GetItemAsync<TOut>(string key);
	Task<string?> GetItemAsStringAsync(string key);
	Task SetItemAsync<TIn>(string key, TIn value);
	Task SetItemAsync<TIn>(string key, TIn value, CancellationToken cancellationToken);
	Task SetItemAsStringAsync(string key, string value);
	Task SetItemAsStringAsync(string key, string value, CancellationToken cancellationToken);
}