﻿using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Services;

public interface IFileStorageService
{
	Task<bool> CopyFile(string sourceContainer, string sourceFilePath, string destContainer, string destFilePath, CancellationToken cancellationToken);

	Task<bool> DeleteDirectory(string container, string relativePath, CancellationToken cancellationToken);

	Task<bool> DeleteFile(string container, string relativePath, CancellationToken cancellationToken);

	Task<CommandResult> SaveFile(string container, string relativePath, string contentType, byte[] data, CancellationToken cancellationToken);

	Task<CommandResult> SaveFile(string container, string relativePath, string contentType, Stream data, CancellationToken cancellationToken);
}
