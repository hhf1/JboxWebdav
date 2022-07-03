﻿using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using JboxWebdav.Server.Jbox;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace NWebDav.Server.Handlers
{
    /// <summary>
    /// Implementation of the DELETE method.
    /// </summary>
    /// <remarks>
    /// The specification of the WebDAV DELETE method can be found in the
    /// <see href="http://www.webdav.org/specs/rfc2518.html#METHOD_DELETE">
    /// WebDAV specification
    /// </see>.
    /// </remarks>
    public class DeleteHandler : IRequestHandler
    {
        /// <summary>
        /// Handle a DELETE request.
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context of the request.
        /// </param>
        /// <param name="store">
        /// Store that is used to access the collections and items.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous DELETE operation. The task
        /// will always return <see langword="true"/> upon completion.
        /// </returns>
        public async Task<bool> HandleRequestAsync(IHttpContext httpContext, IStore store)
        {
            // Obtain request and response
            var request = httpContext.Request;
            var response = httpContext.Response;

            // Keep track of all errors
            var errors = new UriResultCollection();

            // We should always remove the item from a parent container
            var splitUri = RequestHelper.SplitUri(request.Url);

            // Obtain parent collection
            var parentCollectionUri = UriHelper.GetPathFromUri(splitUri.CollectionUri);

            // Obtain the item that actually is deleted
            var deleteItemUri = UriHelper.Combine(parentCollectionUri, splitUri.Name);

            // Delete item
            JboxMoveItemInfo res = null;
            res = JboxService.DeleteJboxItem(deleteItemUri);
            if (res.success)
            {
                response.SetStatus(DavStatusCode.Ok);
                return true;
            }
            if (res.Code == "source not found")
            {
                // Item not found
                response.SetStatus(DavStatusCode.NotFound);
                return true;
            }
            response.SetStatus(DavStatusCode.BadRequest);
            return true;
        }

        private async Task<DavStatusCode> DeleteItemAsync(IStoreCollection collection, string name, IStoreItem deleteItem, IHttpContext httpContext, Uri baseUri)
        {
            //if (deleteItem is IStoreCollection deleteCollection)
            //{
            //    // Determine the new base URI
            //    var subBaseUri = UriHelper.Combine(baseUri, name);

            //    // Delete all entries first
            //    foreach (var entry in await deleteCollection.GetItemsAsync(httpContext).ConfigureAwait(false))
            //        await DeleteItemAsync(deleteCollection, entry.Name, entry, httpContext, subBaseUri).ConfigureAwait(false);
            //}

            // Attempt to delete the item
            return await collection.DeleteItemAsync(name, httpContext).ConfigureAwait(false);
        }
    }
}
