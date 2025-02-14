using Audit.Core;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Audit.Net.PoC
{
    public class BulkTests
    {
        public IList<Message> GetMessages()
        {
            var lorem = new Bogus.DataSets.Lorem();
            IList<Message> messages = new List<Message>();
            for (int i = 0; i < 5000; i++)
            {
                Message message = new Message
                {
                    MessageId = Guid.NewGuid(),
                    Column1 = lorem.Sentence(4, 10) + i,
                    Column2 = lorem.Sentence(4, 10) + i,
                    Column3 = lorem.Sentence(4, 10) + i,
                    Column4 = lorem.Sentence(4, 10) + i,
                    Column5 = lorem.Sentence(4, 10) + i,
                    Column6 = lorem.Sentence(4, 10) + i,
                    Column7 = lorem.Sentence(4, 10) + i,
                    Column8 = lorem.Sentence(4, 10) + i,
                    Column9 = lorem.Sentence(4, 10) + i,
                    Column10 = lorem.Sentence(4, 10) + i,
                    Column11 = lorem.Sentence(4, 10) + i,
                    Column12 = lorem.Sentence(4, 10) + i,
                    Column13 = lorem.Sentence(4, 10) + i,
                    Column14 = lorem.Sentence(4, 10) + i,
                    Column15 = lorem.Sentence(4, 10) + i,
                    Column16 = lorem.Sentence(4, 10) + i,
                    Column17 = lorem.Sentence(4, 10) + i,
                    Column18 = lorem.Sentence(4, 10) + i,
                    Column19 = lorem.Sentence(4, 10) + i,
                    Column20 = lorem.Sentence(4, 10) + i,
                    Column21 = lorem.Sentence(4, 10) + i,
                    Column22 = lorem.Sentence(4, 10) + i,
                    Column23 = lorem.Sentence(4, 10) + i,
                    Column24 = lorem.Sentence(4, 10) + i,
                    Column25 = lorem.Sentence(4, 10) + i,
                    Column26 = lorem.Sentence(4, 10) + i,
                    Column27 = lorem.Sentence(4, 10) + i,
                    Column28 = lorem.Sentence(4, 10) + i,
                    Column29 = lorem.Sentence(4, 10) + i,
                    Column30 = lorem.Sentence(4, 10) + i,
                };

                messages.Add(message);
            }
            return messages;
        }

        public BulkTests()
        {
            using (var context = new TestContext())
            {
                var result = context.Database.EnsureCreated();
            }
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
        }

        [Fact]
        public async Task InsertTestAsync()
        {
            var messageId = Guid.NewGuid();

            Audit.EntityFramework.Configuration.Setup()
                .ForContext<TestContext>(_ => _.EarlySavingAudit(true));

            Audit.Core.Configuration.Setup()
                .UseEntityFramework(ef => ef
                    .AuditTypeExplicitMapper(m => m
                        .Map<Message, MessageAudit>()
                        .AuditEntityAction<MessageAudit>((auditEvent, eventEntry, entity) =>
                        {
                            entity.AuditData = eventEntry.ToJson();
                            entity.AuditTimestamp = DateTimeOffset.UtcNow;
                            entity.AuditAction = eventEntry.Action;
                        })
                    )
                );

            TestContext context = null;
            try
            {
                context = new TestContext();
                using (var tran = context.Database.BeginTransaction())
                {
                    foreach (var item in GetMessages())
                    {
                        await context.AddAsync(item);
                    }

                    await context.SaveChangesAsync().ConfigureAwait(false);
                    await tran.CommitAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task InsertTestBatchAsync()
        {
            Audit.EntityFramework.Configuration.Setup()
                 .ForContext<TestContext>(_ => _.EarlySavingAudit(true));

            Audit.Core.Configuration.Setup()
                .UseEntityFramework(ef => ef
                    .AuditTypeExplicitMapper(m => m
                        .Map<Message, MessageAudit>()
                        .AuditEntityAction<MessageAudit>((auditEvent, eventEntry, entity) =>
                        {
                            entity.AuditData = eventEntry.ToJson();
                            entity.AuditTimestamp = DateTimeOffset.UtcNow;
                            entity.AuditAction = eventEntry.Action;
                        })
                    )
                );

            TestContext context = new TestContext();

            try
            {
                int count = 0;
                foreach (var entityToInsert in GetMessages())
                {
                    ++count;
                    context = await AddToContextAsync(context, entityToInsert, count, 100, true).ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            catch (Exception ex)
            {
                var a = ex;
                throw;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }

        }

        private async Task<TestContext> AddToContextAsync(TestContext context, Message entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<Message>().Add(entity);
            if (count % commitCount == 0)
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
                if (recreateContext)
                {
                    context.Dispose();
                    context = new TestContext();
                }
            }

            return context;
        }

        [Fact]
        public async Task UpdateTestAsync()
        {
            var messageId = Guid.NewGuid();

            Audit.EntityFramework.Configuration.Setup()
                .ForContext<TestContext>(_ => _.EarlySavingAudit(true));

            Audit.Core.Configuration.Setup()
                .UseEntityFramework(ef => ef
                    .AuditTypeExplicitMapper(m => m
                        .Map<Message, MessageAudit>()
                        .AuditEntityAction<MessageAudit>((auditEvent, eventEntry, entity) =>
                        {
                            entity.AuditData = eventEntry.ToJson();
                            entity.AuditTimestamp = DateTimeOffset.UtcNow;
                            entity.AuditAction = eventEntry.Action;
                        })
                    )
                );

            TestContext context = null;
            try
            {
                context = new TestContext();
                using (var tran = context.Database.BeginTransaction())
                {
                    var messages = await context.Set<Message>().ToListAsync().ConfigureAwait(false);

                    foreach (var item in messages)
                    {
                        item.Column1 = item.Column1 + "updated+3";
                        context.Update(item);
                    }

                    await context.SaveChangesAsync().ConfigureAwait(false);
                    await tran.CommitAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task UpdateTestBatchAsync()
        {
            Audit.EntityFramework.Configuration.Setup()
                 .ForContext<TestContext>(_ => _.EarlySavingAudit(true));

            Audit.Core.Configuration.Setup()
                .UseEntityFramework(ef => ef
                    .AuditTypeExplicitMapper(m => m
                        .Map<Message, MessageAudit>()
                        .AuditEntityAction<MessageAudit>((auditEvent, eventEntry, entity) =>
                        {
                            entity.AuditData = eventEntry.ToJson();
                            entity.AuditTimestamp = DateTimeOffset.UtcNow;
                            entity.AuditAction = eventEntry.Action;
                        })
                    )
                );


            TestContext context = null;
            try
            {
                context = new TestContext();

                context.ChangeTracker.AutoDetectChangesEnabled = false;
                var messages = await context.Set<Message>().ToListAsync().ConfigureAwait(false);
                int count = 0;
                foreach (var entityToUpdate in messages)
                {
                    entityToUpdate.Column1 = entityToUpdate + "updatedBatch-2";
                    ++count;
                    context = await UpdateToContextAsync(context, entityToUpdate, count, 100, true).ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                var a = ex;
                throw;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        private async Task<TestContext> UpdateToContextAsync(TestContext context, Message entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<Message>().Update(entity);
            if (count % commitCount == 0)
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
                if (recreateContext)
                {
                    context.Dispose();
                    context = new TestContext();
                }
            }

            return context;
        }
    }
}
