namespace Schema.NET.Test
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Xunit;

    public class MixedTypesTest
    {
        private readonly Book book =
            new Book()
            {
                Id = new Uri("http://example.com/book/1"),
                Author = new List<object>()
                {
                    new Organization()
                    {
                        Name = "Penguin",
                    },
                    new Person()
                    {
                        Name = "J.D. Salinger",
                    },
                },
            };

        private readonly string json =
            "{" +
                "\"@context\":\"http://schema.org\"," +
                "\"@type\":\"Book\"," +
                "\"@id\":\"http://example.com/book/1\"," +
                "\"author\":[" +
                    "{" +
                        "\"@type\":\"Organization\"," +
                        "\"name\":\"Penguin\"" +
                    "}," +
                    "{" +
                        "\"@type\":\"Person\"," +
                        "\"name\":\"J.D. Salinger\"" +
                    "}" +
                "]" +
            "}";

        [Fact]
        public void DeserializeObject_WithCollectionOfMixedTypes_ReturnsObjectWithBothTypes()
        {
            var book = JsonConvert.DeserializeObject<Book>(this.json, TestDefaults.DefaultJsonSerializerSettings);

            Assert.True(book.Author.HasValue);
            Assert.True(book.Author.Value.HasValue1);
            Assert.True(book.Author.Value.HasValue2);
            Assert.True(book.Author.Value.Value1.HasOne);
            Assert.True(book.Author.Value.Value2.HasOne);
            Assert.False(book.Author.Value.Value1.HasMany);
            Assert.False(book.Author.Value.Value2.HasMany);
            Assert.Single(book.Author.Value.Value1);
            Assert.Single(book.Author.Value.Value2);

            List<IPerson> people = book.Author;
            List<IOrganization> organizations = book.Author;

            var person = Assert.Single(people);
            var organization = Assert.Single(organizations);
            Assert.Equal("J.D. Salinger", person.Name);
            Assert.Equal("Penguin", organization.Name);
        }

        [Fact]
        public void DeserializeObject_DeserializesBankAccountTypeToStringAndUri_BankAccountTypeHasStringAndUriValues()
        {
            var json =
                @"{" +
                    "\"@context\":\"http://schema.org\"," +
                    "\"@type\":\"BankAccount\"," +
                    "\"bankAccountType\":[" +
                        "\"http://example.com/1\"," +
                    "]" +
                "}";

            var bankAccount = JsonConvert.DeserializeObject<BankAccount>(json, TestDefaults.DefaultJsonSerializerSettings);

            Assert.True(bankAccount.BankAccountType.HasValue);
            Assert.Equal(
                new List<object>() { "http://example.com/1", new Uri("http://example.com/1") },
                bankAccount.BankAccountType.Value);
        }

        [Fact]
        public void ToString_Book_MatchesExpectedJson() =>
            Assert.Equal(this.json, this.book.ToString());

        [Fact]
        public void ToString_SerializesBankAccountTypeFromStringAndUri_BankAccountTypeHasTwoValues()
        {
            var bankAccount = new BankAccount()
            {
                BankAccountType = new List<object>()
                {
                    "http://example.com/1",
                    new Uri("http://example.com/2")
                }
            };
            var json =
                @"{" +
                    "\"@context\":\"http://schema.org\"," +
                    "\"@type\":\"BankAccount\"," +
                    "\"bankAccountType\":[" +
                        "\"http://example.com/1\"," +
                        "\"http://example.com/2\"" +
                    "]" +
                "}";
            Assert.Equal(json, bankAccount.ToString());
        }
    }
}
