DELIMITER $$

DROP PROCEDURE IF EXISTS `pos_p_getbuyoffer` $$
CREATE DEFINER=`textaltpos`@`%` PROCEDURE `pos_p_getbuyoffer`(IN BarCodeX varchar(40))
BEGIN
if f_IsIsbn( BarCodeX )
  then
    SET @PriceRatio = (select `value` from sysconfig where `key`='priceratio');
    set @WholeSalePercent = (select `value` from sysconfig where `key`='wholesalepercent');
    set @Isbn9 = f_ChangeToIsbn9( BarCodeX );
    
    select *, usedoffer * @WholeSalePercent as usedoffer from wholesale_t_wholesalebook a
    join wholesale_t_wholesalers b on a.wholesaler_key = b.pk
    where isbn9 = @Isbn9 and curdate() < end_date
    order by usedoffer*commission desc limit 1;
    
    
    select  ifnull(b.title,a.title) as title,
            ifnull(b.author,a.author) as author,
            ifnull(b.publisher,a.publisher) as publisher,
            ifnull(b.edition,a.edition) as edition,
            ifnull(b.reqd,a.required) as reqd,
            a.new_price as bn_new_pr,
            a.used_price as bn_used_pr,
            ifnull( round( b.newpr * 0.999999999 ), round( a.new_price * @PriceRatio) ) as new_pr,
            ifnull(round( b.usedpr * 0.999999999 ), round( a.used_price * @PriceRatio) ) as used_pr,
            ifnull(b.ShouldBuy, a.Shouldbuy ) as ShouldBuy,
            ifnull(b.ShouldSell, a.ShouldSell ) as ShouldSell,
            ifnull(b.ShouldOrder, a.ShouldOrder ) as ShouldOrder,
            MaxEnrol,
            CurrentEnrol,
            WaitlistEnrol,
            ifnull(b.DesiredStock, a.DesiredStock ) as DesiredStock,
            BuyOffer
            from iupui_t_books a left join pos_t_items b on a.isbn9 = b.isbn9 where a.isbn9 = @Isbn9 and ( a.maxenrol > 0 or b.desiredstock > 0 );
            

  else
    select * from pos_t_items where BarCode = BarCodex;
  end if;
END $$

DELIMITER ;














DELIMITER $$

DROP PROCEDURE IF EXISTS `pos_p_getbuyoffer` $$
CREATE  PROCEDURE `pos_p_getbuyoffer`(IN BarCodeX varchar(40))
BEGIN
if f_IsIsbn( BarCodeX )
  then
    SET @PriceRatio = (select `value` from sysconfig where `key`='priceratio');
    set @WholeSalePercent = (select `value` from sysconfig where `key`='wholesalepercent');

    set @Isbn9 = f_ChangeToIsbn9( BarCodeX );

    -- select wholesale offer table
    select *, usedoffer * @WholeSalePercent as usedoffer from wholesale_t_wholesalebook a
    join wholesale_t_wholesalers b on a.wholesaler_key = b.pk
    where isbn9 = @Isbn9 and curdate() < end_date
    order by usedoffer*commission desc limit 1;


    -- select IUPUI offer table, or override ws offer

    if ( select count(*) from pos_t_items ) > 0
      then

        -- if it has a pos_t_items, send it
        select  ifnull(b.title,ifnull(a.title,'no title')) as title,
            ifnull(b.author,ifnull(a.author,'no author')) as author,
            ifnull(b.publisher,ifnull(a.publisher,'no publisher')) as publisher,
            ifnull(b.edition,ifnull(a.edition,'no publisher')) as edition,
            ifnull(b.reqd,ifnull(a.reqd,0)) as reqd,
            ifnull(a.new_price,0) as bn_new_pr,
            ifnull(a.used_price,0) as bn_used_pr,
            ifnull( round( b.newpr * 0.999999999 ), 99999 ) as new_pr,
            ifnull( round( b.usedpr * 0.999999999 ), 99999 ) as used_pr,
            ifnull(b.ShouldBuy,0 ) as ShouldBuy,
            ifnull(b.ShouldSell,0 ) as ShouldSell,
            ifnull(b.ShouldOrder,0 ) as ShouldOrder,
            0 as MaxEnrol,
            0 as CurrentEnrol,
            0 as WaitlistEnrol,
            ifnull(b.DesiredStock,0 ) as DesiredStock,
            ifnull( round( b.BuyOffer * 0.999999) ,0) as BuyOffer
            from pos_t_items b
            left join iupui_t_books a on a.isbn = b.isbn9
            where b.isbn9 = @Isbn9;


      else

      -- if not, then select only if it has a wholesale record

            select
            ifnull(a.title,'') as title,
            ifnull(a.author,'') as author,
            ifnull(a.publisher,'') as publisher,
            ifnull(a.edition,'') as edition,
            ifnull(b.reqd,0) as reqd,
            a.new_price as bn_new_pr,
            a.used_price as bn_used_pr,
            ifnull( round( a.new_price * @PriceRatio), 0 ) as new_pr,
            ifnull( round( a.used_price * @PriceRatio), 0 ) as used_pr,
            1 as ShouldBuy,
            1 as ShouldSell,
            1 as ShouldOrder,
            MaxEnrol,
            CurrentEnrol,
            WaitlistEnrol,
            a.DesiredStock as DesiredStock,
            round( BuyOffer * 0.99999999 ) as BuyOffer
            from wholesale_t_wholesalebook b
            join iupui_t_books a on b.isbn9 = a.isbn9
            where b.isbn9 = @Isbn9 and curdate() < b.end_date limit 1;

      end if;

  else
    select * from pos_t_items where BarCode = BarCodex;
  end if;
END $$

DELIMITER ;



