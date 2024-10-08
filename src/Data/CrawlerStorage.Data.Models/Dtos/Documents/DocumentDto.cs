﻿namespace CrawlerStorage.Data.Models.Dtos.Documents;

public class DocumentDto
{
    public int Id { get; set; }

    public Guid Identifier { get; set; }

    public string Name { get; set; }

    public string Format { get; set; }

    public int Order { get; set; }

    public string Url { get; set; }

    public string Encoding { get; set; }

    public string Operation { get; set; }

    public string MD5 { get; set; }

    public int GroupId { get; set; }

    public byte[] Content { get; set; }
}