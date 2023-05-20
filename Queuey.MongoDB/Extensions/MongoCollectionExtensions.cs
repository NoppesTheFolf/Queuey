using MongoDB.Driver;

namespace Noppes.Queuey.MongoDB.Extensions;

internal static class MongoCollectionExtensions
{
    public static async Task<T> FindOneOrDefaultAsync<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter)
    {
        var cursor = await collection.FindAsync(filter);
        var entity = await cursor.FirstOrDefaultAsync();

        return entity;
    }
}
