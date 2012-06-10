<?php


	date_default_timezone_set('Europe/Kiev');
    
    require_once $_SERVER['DOCUMENT_ROOT'] . "/libs/globals/global.web.php";
    
    /*require_once $SERVER['DOCUMENT_ROOT'] . "/libs/configs/cfg.db.php";

    function __autoload($class_name)
    {
        $fn = strtolower("D:/hshome/marihooan/pobutteh.com.ua/libs/classes/".$class_name.".class.php");
        require_once $fn;
        
    }

    $db_lib = new DataBase();
    */
    $db_lib->Connect();
    $lastArtsTable = $db_lib->GetTable("SELECT
    ishop_articles.AID, ishop_articles.AIMAGE, ishop_articles.ADATEUPDATE, ishop_articles.ADESC, ishop_articles.ANAME, ishop_categories.CATID, ishop_categories.CATNAME, ishop_developers.DEVNAME, ishop_developers.DEVID
    FROM ishop_articles
    LEFT OUTER JOIN ishop_categories ON ishop_articles.CATID = ishop_categories.CATID
    LEFT OUTER JOIN ishop_developers ON ishop_articles.DEVID = ishop_developers.DEVID
    ORDER BY ishop_articles.AID DESC");
    $sql = "
    SELECT
        allCats.CATID,
        allCats.CATNAME,
        allCats.DATEUPDATED as `cDate`,
        shopData.DEVID,
        shopData.DEVNAME,
        shopData.DATEUPDATED as `dDate`, (
            SELECT COUNT( * )
            FROM ishop_articles AS  `ctArts`
            WHERE ctArts.CATID = allCats.CATID
            AND ctArts.DEVID = shopData.DEVID
        ) AS  `itemsCount`
    FROM ishop_categories AS  `allCats`
    LEFT JOIN (
        SELECT
            ishop_articles.CATID, ishop_categories.CATNAME, ishop_articles.DEVID, ishop_developers.DEVNAME,
            ishop_developers.DATEUPDATED
        FROM ishop_articles
        LEFT JOIN ishop_categories ON ishop_articles.CATID = ishop_categories.CATID
        LEFT JOIN ishop_developers ON ishop_articles.DEVID = ishop_developers.DEVID
    ) AS  `shopData` ON allCats.CATID = shopData.CATID";

    $shopStruct = $db_lib->PerformCommand($sql);
    $categories = array();
    $i = 0;
    foreach ($shopStruct as $rowS => $valS){
        if (!isset($categories[$valS['CATID']]))
        {
            $categories[$valS['CATID']] = array('__name__' => $valS['CATNAME'], '__date__' => $valS['cDate'], '__origins__' => array());
        }
        if (isset($valS['DEVID']) && !isset($categories[$valS['CATID']]['__origins__'][$valS['DEVID']]))
            $categories[$valS['CATID']]['__origins__'][$valS['DEVID']] = array('__name__' => $valS['DEVNAME'], '__date__' => $valS['dDate'], '__total__' => $valS['itemsCount']);
    }

    $db_lib->Close();
    
    $now = date('Y-m-d');

    $year = substr($now,0,4); //work out the year  
    $mon  = substr($now,5,2); //work out the month  
    $day  = substr($now,8,2); //work out the day  

    /*display the date in the format Google expects: 
    2006-01-29 for example*/  

    $displaydate = ''.$year.'-'.$mon.'-'.$day.''; 

    /*$output = '<pre><?xml version="1.0" encoding="UTF-8"?>*/
    $output = '<?xml version="1.0" encoding="UTF-8"?>
    <urlset xmlns="http://www.google.com/schemas/sitemap/0.84">';


    $url_product = SITEURL;

    foreach ($categories as $ckey => $row)
    {
        // categories
        //your url-product as we worked out in #4  
        $url_product = SITEURL . 'category/' . $ckey . '/' . $row['__name__'] . '.html';
        $url_product = str_replace(' ', '_', $url_product);

        //you can assign whatever changefreq and priority you like 
        $output .= '
        <url>  
            <loc>'.($url_product).'</loc>  
            <lastmod>'.substr($row['__date__'], 0, 10).'</lastmod>  
            <changefreq>monthly</changefreq>  
            <priority>0.8</priority>  
        </url>';

        // origins
        if (isset($row['__origins__']) && count($row['__origins__']))
            foreach ($row['__origins__'] as $okey => $oit)
            {
                // developers
                //your url-product as we worked out in #4  
                //$url_product = 'http://pobutteh.com.ua/shop.php?cat=' . $ckey . '&amp;dev=' . $okey;  
                $url_product = SITEURL . 'category/' . $ckey . '/' . $row['__name__'] . '/origin/' . $okey . '/' . $oit['__name__'] . '.html';
                $url_product = str_replace(' ', '_', $url_product);
                //you can assign whatever changefreq and priority you like 
                $output .= '
                <url>  
                    <loc>'.($url_product).'</loc>  
                    <lastmod>'.substr($oit['__date__'], 0, 10).'</lastmod>  
                    <changefreq>monthly</changefreq>  
                    <priority>0.8</priority>  
                </url>';
            }
    }

    foreach ($lastArtsTable as $key => $line)
    {
        // products
        //your url-product as we worked out in #4  
        $url_product = SITEURL . 'product/' . $line['AID'] . '/' . $line['ANAME'] . '.html';  
        $url_product = str_replace(' ', '_', $url_product);

        //you can assign whatever changefreq and priority you like 
        $output .= '
        <url>  
            <loc>'.($url_product).'</loc>  
            <lastmod>'.substr($line['ADATEUPDATE'], 0, 10).'</lastmod>  
            <changefreq>daily</changefreq>  
            <priority>0.9</priority>  
        </url>';  
    }
    $output .= "</urlset>";
    //$output .= "</urlset></pre>";

    // —ообщ€ем браузеру что передаем XML
    header("Content-type: text/xml; charset=Windows-1251");

    echo $output;
?>
