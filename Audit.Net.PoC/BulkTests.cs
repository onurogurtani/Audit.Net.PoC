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
            IList<Message> messages = new List<Message>();
            for (int i = 0; i < 50; i++)
            {
                Message message = new Message
                {
                    MessageId = Guid.NewGuid(),
                    Column1 = "Column1" + i,
                    Column2 = "Column2" + i,
                    Column3 = "Column3" + i,
                    Column4 = "Column4" + i,
                    Column5 = "Column5" + i,
                    Column6 = "Column6" + i,
                    Column7 = "Column7" + i,
                    Column8 = "Column8" + i,
                    Column9 = "Column9" + i,
                    Column10 = "Column10" + i,
                    Column11 = "Column11" + i,
                    Column12 = "Column12" + i,
                    Column13 = "Column13" + i,
                    Column14 = "Column14" + i,
                    Column15 = "Column15" + i,
                    Column16 = "Column16" + i,
                    Column17 = "Column17" + i,
                    Column18 = "Column18" + i,
                    Column19 = "Column19" + i,
                    Column20 = "Column20" + i,
                    Column21 = "Column21" + i,
                    Column22 = "Column22" + i,
                    Column23 = "Column23" + i,
                    Column24 = "Column24" + i,
                    Column25 = "Column25" + i,
                    Column26 = "Column26" + i,
                    Column27 = "Column27" + i,
                    Column28 = "Column28" + i,
                    Column29 = "Column29" + i,
                    Column30 = "Column30" + i,
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

                foreach (var item in GetMessages())
                {
                    await context.AddAsync(item);
                }

                await context.SaveChangesAsync();
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


            TestContext context = null;
            try
            {
                context = new TestContext();

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

                var messages = await context.Set<Message>().ToListAsync().ConfigureAwait(false);

                foreach (var item in messages)
                {
                    item.Column1 = item.Column1 + "updated+3";
                    context.Update(item);
                }

                await context.SaveChangesAsync();
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
