from elasticsearch import Elasticsearch
from pprint import pprint
from theatre import* 


es = Elasticsearch("http://localhost:9200")

def ensure_index_exists():
    settings = {
    "settings": {
        "analysis": {
            "char_filter": {
                "remove_digits": {   #symbol filter 
                    "type": "pattern_replace",
                    "pattern": "\\d+",
                    "replacement": ""
                }
            },
            "filter": {
                "short_long_token_filter": { #token filter 
                    "type": "length",
                    "min": 3,
                    "max": 18
                }
            },
            "analyzer": {
                "custom_analyser": {
                    "type": "custom",
                    "char_filter": ["remove_digits"],
                    "tokenizer": "standard",
                    "filter": ["lowercase", "short_long_token_filter"]
                }
            }
        }
    },
        "mappings": {
            "properties": {
                "name": {
                    "type": "keyword",
                },
                "location": {
                    "type": "keyword"
                },
                "capacity": {
                    "type": "integer"
                },
                "genres": {
                    "type": "keyword"
                },
                "description": { 
                    "type": "text",
                    "analyzer": "standard"  # standart analyser (4)
                },
                "director_bio": { 
                    "type": "text",
                    "analyzer": "english"  # english analyser (5)
                },
                "critic_reviews": { 
                    "type": "text",
                    "analyzer": "custom_analyser"  # custom analyser (6)
                }
            }
        }
    }

    es.indices.create(index="theatres_2", body=settings, ignore=400)  
    print("Index 'theatres_2' created and updated.")


def add_theatre():
    name = input("Enter the name of the theater: ")
    location = input("Enter theater location:")
    capacity = int(input("Enter theater capacity: "))
    genres = input("Enter theater genres separated by commas: ").split(",")
    description= input("Enter theatre description:")
    director_bio= input("Enter the theater director's biography:")
    critic_reviews= input("Enter a review of critics about the theater:")

    theatre = Theatre(name, location, capacity, genres, description, director_bio, critic_reviews)
    response = es.index(index="theatres_2", document=theatre.to_dict())
    print("Theater added:", response['_id'])


def delete_theatre():
    theatre_id = input("Enter theater ID to delete:")
    response = es.delete(index="theatres_2", id=theatre_id)
    print("Deleted:", response['result'])


def get_all_theatres():
    try:
        response = es.search(index="theatres_2", query={"match_all": {}})
        theatres = [hit['_source'] for hit in response['hits']['hits']]
        for theatre in theatres:
            print(theatre)

    except Exception as e:
        print("Error while receiving data:", e)


def search_by_name():
    search_term= input("Enter the name of the theater that you want to find: ")
    # fuzzy search for name(keyword)
    try:
        response = es.search(index="theatres_2", query={
            "fuzzy": {
                "name": {
                    "value": search_term
                }
            }
        })

        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error during search: {e}")



def search_by_location():
    search_location = input("Enter theater location, that you want to find:")
    try:
        #  term search for location(keyword)
        response = es.search(index="theatres_2", query={
            "term": {
                "location.keyword": {
                    "value": search_location
                }
            }
        })

        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error while serching: {e}")


def search_by_capacity():
    min_capacity = int(input("Enter min theater capacity: "))
    max_capacity = int(input("Enter max theater capacity: "))
    try:
        # range search for capacity(int)
        response = es.search(index="theatres_2", query={
            "range": {
                "capacity": {
                    "gte": min_capacity,
                    "lte": max_capacity
                }
            }
        })

        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error while searching: {e}")


def search_by_genres():
    genres_list = input("Enter theater genres, that you want to find, separated by commas: ").split(",")
    try:
        #  terms search for genres
        response = es.search(index="theatres_2", query={
            "terms": {
                "genres": genres_list
            }
        })

        print(f"Found {response['hits']['total']['value']} documents:")
        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error while searching: {e}")


def search_by_description():
    search_text= input("Enter search request:")
    try:
        query = {
            "query": {
                "match": {
                    "description": {
                        "query": search_text 
                    }
                }
            }
        }

        response = es.search(index="theatres_2", body=query)
        print(f"Found {response['hits']['total']['value']} documents:")
        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error while searching: {e}")


def search_by_derictor_bio():
    search_text= input("Enter search request:")
    try:
        query = {
            "query": {
                "match": {
                    "director_bio": {
                        "query": search_text 
                    }
                }
            }
        }

        response = es.search(index="theatres_2", body=query)
        print(f"Found {response['hits']['total']['value']} documents:")
        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error while searching: {e}")


def search_by_critic_reviews():
    search_text= input("Enter search request:")
    try:
        query = {
            "query": {
                "match": {
                    "critic_reviews": {
                        "query": search_text 
                    }
                }
            }
        }

        response = es.search(index="theatres_2", body=query)
        print(f"Found {response['hits']['total']['value']} documents:")
        for hit in response['hits']['hits']:
            print(hit['_source'])

    except Exception as e:
        print(f"Error while searching: {e}")



def main():
    ensure_index_exists()
    actions = {
        '1': add_theatre,
        '2': delete_theatre,
        '3': get_all_theatres,
        '4': search_by_name,
        '5': search_by_location,
        '6': search_by_capacity,
        '7': search_by_genres,
        '8': search_by_description,
        '9': search_by_derictor_bio,
        '10': search_by_critic_reviews

    }

    while True:
        print("\nChoose an action:")
        print("1 - Add new theatre")
        print("2 - Delete theatre")
        print("3 - Get all theatres")
        print("4 - Search theatre by name")
        print("5 - Search theatre by location")
        print("6 - Search theatre by capacity")
        print("7 - Search theatre by genres")
        print("8 - Search theatre by description")
        print("9 - Search theatre by director bio")
        print("10 - Search theatre by critic reviews")
        print("0 - Exit")

        choice = input("> ")
        if choice == '0':
            break
        action = actions.get(choice)
        if action:
            action()
        else:
            print("Unknown command, try again.")


if __name__ == "__main__":
    main()


